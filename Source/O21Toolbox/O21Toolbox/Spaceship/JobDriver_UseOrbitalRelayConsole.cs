using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace O21Toolbox.Spaceship
{
    public class JobDriver_UseOrbitalRelayConsole : JobDriver
    {
        public TargetIndex orbitalRelayConsoleIndex = TargetIndex.A;
        public AirStrikeDef selectedStrikeDef = null;
        public TimeSpeed previousTimeSpeed = TimeSpeed.Paused;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Building_OrbitalRelay orbitalRelay = this.TargetThingA as Building_OrbitalRelay;

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(delegate ()
            {
                return (orbitalRelay.canUseConsoleNow == false);
            });

            Toil faceConsoleToil = new Toil()
            {
                initAction = () =>
                {
                    this.GetActor().rotationTracker.FaceCell(orbitalRelay.Position);
                }
            };
            yield return faceConsoleToil;

            Toil callMiningCoToil = new Toil()
            {
                initAction = () =>
                {
                    DiaNode mainDiaNode = new DiaNode("\"MiningCo. here!\n"
                        + "What do you want, partner?\"");

                    // Cargo supply request option.
                    DiaOption cargoSupplyDiaOption = GetCargoSupplyDiaOption(orbitalRelay);
                    mainDiaNode.options.Add(cargoSupplyDiaOption);

                    // Recruitment supply request option.
                    DiaOption recruitmentSupplyDiaOption = GetRecruitmentSupplyDiaOption(orbitalRelay);
                    mainDiaNode.options.Add(recruitmentSupplyDiaOption);

                    // Air support request option.
                    DiaOption airSupportDiaOption = GetAirSupportDiaOption(orbitalRelay, mainDiaNode);
                    mainDiaNode.options.Add(airSupportDiaOption);

                    // Disconnect option.
                    DiaOption disconnectDiaOption = new DiaOption("Disconnect");
                    disconnectDiaOption.resolveTree = true;
                    mainDiaNode.options.Add(disconnectDiaOption);

                    Find.WindowStack.Add(new Dialog_NodeTree(mainDiaNode, true, true, "-- Comlink with MiningCo. --"));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return callMiningCoToil;

            yield return Toils_Reserve.Release(orbitalRelayConsoleIndex);
        }

        public DiaOption GetCargoSupplyDiaOption(Building_OrbitalRelay orbitalRelay)
        {
            DiaOption cargoSupplyDiaOption = new DiaOption("Request cargo supply (" + Util_Spaceship.cargoSupplyCostInSilver + " silver)");
            Building_LandingPad landingPad = Util_LandingPad.GetBestAvailableLandingPad(this.Map);
            if (Find.TickManager.TicksGame <= Util_Misc.Partnership.nextRequestedSupplyMinTick[this.Map])
            {
                cargoSupplyDiaOption.Disable("no available ship for now");
            }
            else if (TradeUtility.ColonyHasEnoughSilver(this.Map, Util_Spaceship.cargoSupplyCostInSilver) == false)
            {
                cargoSupplyDiaOption.Disable("not enough silver");
            }
            else if (landingPad == null)
            {
                cargoSupplyDiaOption.Disable("no available landing pad");
            }
            else if (this.Map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
            {
                cargoSupplyDiaOption.Disable("toxic fallout");
            }
            cargoSupplyDiaOption.action = delegate
            {
                TradeUtility.LaunchSilver(this.Map, Util_Spaceship.cargoSupplyCostInSilver);
                Util_Spaceship.SpawnLandingSpaceship(landingPad, SpaceshipKind.CargoRequested);
            };
            DiaNode supplyShipAcceptedDiaNode = new DiaNode("\"Alright, a supply ship should arrive soon with our best stuff.\n"
                + "Greetings partner!\"\n\n"
                + "-- End of transmission --");
            DiaOption supplyShipAcceptedDiaOption = new DiaOption("OK");
            supplyShipAcceptedDiaOption.resolveTree = true;
            supplyShipAcceptedDiaNode.options.Add(supplyShipAcceptedDiaOption);
            cargoSupplyDiaOption.link = supplyShipAcceptedDiaNode;
            return cargoSupplyDiaOption;
        }

        public DiaOption GetRecruitmentSupplyDiaOption(Building_OrbitalRelay orbitalRelay)
        {
            return null;
        }

        public DiaOption GetAirSupportDiaOption(Building_OrbitalRelay orbitalRelay, DiaNode parentDiaNode)
        {
            DiaOption airSupportDiaOption = new DiaOption("Request air support");
            if (Find.FactionManager.AllFactionsVisibleInViewOrder.Any<Faction>(x => !x.HostileTo(Faction.OfPlayer)))
            {
                // Air support granted.
                DiaNode airSupportGrantedDiaNode = GetAirSupportGrantedDiaNode(parentDiaNode);
                airSupportDiaOption.link = airSupportGrantedDiaNode;
            }
            else
            {
                // Air support denied.
                DiaNode airSupporDeniedDiaNode = GetAirSupporDeniedDiaNode(parentDiaNode);
                airSupportDiaOption.link = airSupporDeniedDiaNode;
            }
            return airSupportDiaOption;
        }

        public DiaNode GetAirSupportGrantedDiaNode(DiaNode parentDiaNode)
        {
            DiaNode airSupportGrantedDiaNode = new DiaNode("\"Your air support request has been approved by my hierarchy.\n"
                + "What kind of strike should we launch?\"");
            foreach (AirStrikeDef strikeDef in DefDatabase<AirStrikeDef>.AllDefsListForReading)
            {
                // Air strike option.
                DiaOption airStrikeDiaOption = new DiaOption(strikeDef.LabelCap + " (" + strikeDef.costInSilver + " silvers)");
                DiaNode airStrikeDetailsDiaNode = GetAirStrikeDetailsDiaNode(airSupportGrantedDiaNode, strikeDef);
                airStrikeDiaOption.link = airStrikeDetailsDiaNode;
                airSupportGrantedDiaNode.options.Add(airStrikeDiaOption);
            }
            // Back option.
            DiaOption airSupportBackDiaOption = new DiaOption("Back");
            airSupportBackDiaOption.link = parentDiaNode;
            airSupportGrantedDiaNode.options.Add(airSupportBackDiaOption);
            return airSupportGrantedDiaNode;
        }

        public DiaNode GetAirStrikeDetailsDiaNode(DiaNode parentNode, AirStrikeDef strikeDef)
        {
            DiaNode airStrikeDetailsDiaNode = new DiaNode(strikeDef.LabelCap + "\n\n"
                   + strikeDef.description + "\n\n"
                   + "Runs number: " + strikeDef.runsNumber + "\n\n"
                   + "Cost: " + strikeDef.costInSilver + " silvers");
            DiaOption airStrikeConfirmDiaOption = new DiaOption("Confirm");
            airStrikeConfirmDiaOption.action = delegate
            {
                this.previousTimeSpeed = Find.TickManager.CurTimeSpeed;
                Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
                this.selectedStrikeDef = strikeDef;
                Util_Misc.SelectAirStrikeTarget(this.Map, SpawnAirStrikeBeacon);
            };
            airStrikeConfirmDiaOption.resolveTree = true;
            if (TradeUtility.ColonyHasEnoughSilver(this.Map, strikeDef.costInSilver) == false)
            {
                airStrikeConfirmDiaOption.Disable("not enough silver");
            }
            airStrikeDetailsDiaNode.options.Add(airStrikeConfirmDiaOption);
            DiaOption airStrikeBackDiaOption = new DiaOption("Back");
            airStrikeBackDiaOption.link = parentNode;
            airStrikeDetailsDiaNode.options.Add(airStrikeBackDiaOption);
            return airStrikeDetailsDiaNode;
        }

        public DiaNode GetAirSupporDeniedDiaNode(DiaNode parentDiaNode)
        {
            DiaNode airSupporDeniedDiaNode = new DiaNode("\"We cannot send you the requested air support.\n"
                + "Damn those pirates, our ammo stock is too low to help you right now.\n"
                + "I hope you will find a solution on your own!\n\n"
                + "Greetings partner!\"\n\n"
                + "-- End of transmission --");
            DiaOption airSupportDeniedDiaOption = new DiaOption("OK");
            airSupportDeniedDiaOption.resolveTree = true;
            airSupporDeniedDiaNode.options.Add(airSupportDeniedDiaOption);
            DiaOption airSupportBackDiaOption = new DiaOption("Back");
            airSupportBackDiaOption.link = parentDiaNode;
            airSupporDeniedDiaNode.options.Add(airSupportBackDiaOption);
            return airSupporDeniedDiaNode;
        }

        public void SpawnAirStrikeBeacon(LocalTargetInfo targetPosition)
        {
            TradeUtility.LaunchSilver(this.Map, this.selectedStrikeDef.costInSilver);
            Building_AirStrikeBeacon airStrikeBeacon = GenSpawn.Spawn(Util_ThingDefOf.AirStrikeBeacon, targetPosition.Cell, this.Map) as Building_AirStrikeBeacon;
            airStrikeBeacon.InitializeAirStrike(targetPosition.Cell, this.selectedStrikeDef);
            airStrikeBeacon.SetFaction(Faction.OfPlayer);
            Messages.Message("Air strike target confirmed!", airStrikeBeacon, MessageTypeDefOf.CautionInput);
            Find.TickManager.CurTimeSpeed = this.previousTimeSpeed;
            Util_Misc.Partnership.nextAirstrikeMinTick[this.Map] = Find.TickManager.TicksGame + Mathf.RoundToInt(selectedStrikeDef.ammoResupplyDays * GenDate.TicksPerDay);
        }

        public DiaOption GetFeePaymentDiaOption(Building_OrbitalRelay orbitalRelay)
        {
            int feeCost = Util_Misc.Partnership.globalGoodwillFeeInSilver + Util_Misc.Partnership.feeInSilver[this.Map];
            DiaOption feePaymentDiaOption = new DiaOption("Pay partnership fee (" + feeCost + ")");
            feePaymentDiaOption.resolveTree = true;
            feePaymentDiaOption.action = delegate
            {
                TradeUtility.LaunchSilver(orbitalRelay.Map, feeCost);
                Util_Misc.Partnership.globalGoodwillFeeInSilver = 0;
                Util_Misc.Partnership.feeInSilver[this.Map] = 0;
                Messages.Message("Partnership fee paid to MiningCo..", MessageTypeDefOf.PositiveEvent);
                if (Util_Faction.MiningCoFaction.GoodwillWith(Faction.OfPlayer) < 0)
                {
                    // Reset goodwill to 0.
                    Util_Faction.AffectGoodwillWith(Util_Faction.MiningCoFaction, Faction.OfPlayer, -Util_Faction.MiningCoFaction.GoodwillWith(Faction.OfPlayer));
                }
            };
            if (TradeUtility.ColonyHasEnoughSilver(this.Map, feeCost) == false)
            {
                feePaymentDiaOption.Disable("not enough silver");
            }
            return feePaymentDiaOption;
        }
    }
}
