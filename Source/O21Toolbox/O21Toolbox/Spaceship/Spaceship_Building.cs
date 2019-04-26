using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class Spaceship_Building : Building, IThingHolder
    {
        // The SpaceshipDef where all modder defined info is pulled from.
        public SpaceshipDef thisSpaceshipDef;

        public int pilotsNumber;
        public int takeOffTick;
        public SpaceshipKind spaceshipKind;
        public TraderKindDef cargoKind;
        public List<Pawn> pawnsAboard = new List<Pawn>();

        public virtual bool takeOffRequestIsEnabled => true;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                // Initialize cargo and destroy roof.
                if(this.things == null)
                {
                    this.things = new ThingOwner<Thing>(this);
                    GenerateThings();

                    DestroyRoof();
                }

                pilotsNumber = thisSpaceshipDef.crewSettings.pilotMax;

                // Generate Pilots.
                for(int pilotIndex = 0; pilotIndex < pilotsNumber; pilotIndex++)
                {
                    Pawn pilot = SpaceshipPawnGenerator.GeneratePawn(GetPilotPawnKind(), this.Map);
                    if(pilot != null)
                    {
                        this.pawnsAboard.Add(pilot);
                    }
                }
            }
        }

        public void InitializeData(Faction faction, int hitPoints, int landingDuration, SpaceshipDef spaceshipDef)
        {
            this.SetFaction(faction);
            this.HitPoints = hitPoints;
            this.takeOffTick = Current.Game.tickManager.TicksGame + landingDuration;
            this.thisSpaceshipDef = spaceshipDef;
            this.cargoKind = GetCargoKind(this.thisSpaceshipDef);
        }

        public PawnKindDef GetPilotPawnKind()
        {
            List<PawnKindDef> pilotList = new List<PawnKindDef>();
            foreach (CrewGroupMaker groupMaker in thisSpaceshipDef.crewGroupMaker.Where(x => x.crewGroupType == spaceshipKind))
            {
                foreach(CrewKindOption crewKind in groupMaker.crewKindOptions)
                {
                    if(crewKind.crewRole == CrewRole.Pilot)
                    {
                        pilotList.Add(crewKind.pawnKind);
                    }
                }
            }

            return pilotList.RandomElement();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Thing landingPad = this.Position.GetFirstThing<Building_LandingPad>(this.Map);
            if(landingPad != null)
            {
                (landingPad as Building_LandingPad).Notify_ShipTakingOff();
            }
            if(mode == DestroyMode.KillFinalize)
            {
                // Spaceship is destroyed.
                SpawnSurvivingPawns();
                SpawnExplosions();
                SpawnFuelPuddleAndFire();

                this.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -30);

                string spaceshipDestroyedText = "-- Comlink with " + this.Faction.def.LabelCap + " --\n\n"
                + "\"We just lost contact with one of our ships in your sector.\n"
                + "Whatever happened, you are held responsible of this loss.\n\n"
                + "-- End of transmission --";
                Find.LetterStack.ReceiveLetter("Spaceship destroyed", spaceshipDestroyedText, LetterDefOf.ThreatSmall, new TargetInfo(this.Position, this.Map));
            }
            else
            {
                // Spaceship taking off.
                foreach(Pawn pawn in this.pawnsAboard)
                {
                    pawn.Destroy();
                }
                this.pawnsAboard.Clear();
                // Spawn taking off spaceship.
                FlyingSpaceshipTakingOff spaceship = ThingMaker.MakeThing(Util_Spaceship.SpaceshipTakingOff) as FlyingSpaceshipTakingOff;
                GenSpawn.Spawn(spaceship, this.Position, this.Map, this.Rotation);
                spaceship.InitializeTakingOffParameters(this.Position, this.Rotation, this.thisSpaceshipDef);
                spaceship.HitPoints = this.HitPoints;
                if(this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_OrbitalRelay>() != null)
                {
                    Messages.Message("A ship is taking off.", spaceship, MessageTypeDefOf.NeutralEvent);
                }
                DestroyRoof();
            }
            this.things.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.takeOffTick, "takeOffTick");
            Scribe_Values.Look<SpaceshipKind>(ref spaceshipKind, "spaceshipKind");
            Scribe_Defs.Look<TraderKindDef>(ref this.cargoKind, "cargoKind");
            Scribe_Deep.Look<ThingOwner>(ref this.things, "things");
            Scribe_Collections.Look<Pawn>(ref this.pawnsAboard, "pawnsAboard", LookMode.Deep);
        }

        public void DestroyRoof()
        {
            foreach (IntVec3 cell in this.OccupiedRect().Cells)
            {
                if (cell.Roofed(this.Map))
                {
                    RoofDef roof = this.Map.roofGrid.RoofAt(cell);
                    if (roof.filthLeaving != null)
                    {
                        FilthMaker.MakeFilth(cell, this.Map, roof.filthLeaving, Rand.RangeInclusive(1, 3));
                    }
                    this.Map.roofGrid.SetRoof(cell, null);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (this.DestroyedOrNull())
            {
                return;
            }
            if ((Find.TickManager.TicksGame >= this.takeOffTick)
                && (this.Map.GameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare) == false))
            {
                this.Destroy(DestroyMode.Vanish);
            }
        }

        public virtual void RequestTakeOff()
        {
            this.takeOffTick = Find.TickManager.TicksGame;
        }

        public TraderKindDef GetCargoKind(SpaceshipKind spaceshipKind)
        {
            TraderKindDef cargokind = Util_TraderKindDefOf.spaceshipCargoPeriodicSupply;
            switch (spaceshipKind)
            {
                case SpaceshipKind.Cargo:
                    cargokind = Util_TraderKindDefOf.spaceshipCargoPeriodicSupply;
                    break;
                case SpaceshipKind.Damaged:
                    cargokind = Util_TraderKindDefOf.spaceshipCargoDamaged;
                    break;
                case SpaceshipKind.DispatcherDrop:
                case SpaceshipKind.DispatcherPick:
                    cargokind = Util_TraderKindDefOf.spaceshipCargoDispatcher;
                    break;
                default:
                    Log.ErrorOnce("Toolbox.Spaceship: unhandled SpaceshipKind (" + this.spaceshipKind.ToString() + ") in Building_Spaceship.GetCargoKind.", 123456781);
                    break;
            }
            return cargokind;
        }

        public void SpawnSurvivingPawns()
        {
            List<Pawn> survivingPawns = new List<Pawn>();
            foreach (Pawn pawn in this.pawnsAboard)
            {
                GenSpawn.Spawn(pawn, this.OccupiedRect().Cells.RandomElement(), this.Map);
                Expedition.RandomlyDamagePawn(pawn, Rand.RangeInclusive(1, 4), Rand.RangeInclusive(4, 16));
                if (pawn.Dead == false)
                {
                    survivingPawns.Add(pawn);
                }
            }
            this.pawnsAboard.Clear();
            if (survivingPawns.Count > 0)
            {
                IntVec3 exitSpot = IntVec3.Invalid;
                bool exitSpotIsValid = RCellFinder.TryFindBestExitSpot(survivingPawns.First(), out exitSpot, TraverseMode.PassAllDestroyableThings);
                if (exitSpotIsValid)
                {
                    LordMaker.MakeNewLord(Util_Faction.MiningCoFaction, new LordJob_ExitMap(exitSpot), this.Map, survivingPawns);
                }
            }
        }

        public void SpawnExplosions()
        {
            for (int explosionIndex = 0; explosionIndex < 5; explosionIndex++)
            {
                GenExplosion.DoExplosion(this.Position + IntVec3Utility.RandomHorizontalOffset(5f), this.Map, Rand.Range(3f, 7f), DamageDefOf.Bomb, this, Rand.Range(8, 45));
            }

        }

        public void SpawnFuelPuddleAndFire()
        {
            for (int fireIndex = 0; fireIndex < 150; fireIndex++)
            {
                IntVec3 spawnCell = this.Position + IntVec3Utility.RandomHorizontalOffset(12f);
                GenSpawn.Spawn(ThingDefOf.Filth_Fuel, spawnCell, this.Map);
                Fire fire = GenSpawn.Spawn(ThingDefOf.Fire, spawnCell, this.Map) as Fire;
                fire.fireSize = Rand.Range(0.25f, 1.25f);
            }
        }

        public void SpawnCargoContent(float integrity)
        {
            for (int thingIndex = 0; thingIndex < this.things.Count; thingIndex++)
            {
                Thing thing = this.things[thingIndex];
                int quantity = Mathf.RoundToInt(thing.stackCount * integrity);
                SpawnItem(thing.def, thing.Stuff, quantity, this.Position, this.Map, 12f);
            }
        }

        public static Thing SpawnItem(ThingDef itemDef, ThingDef stuff, int quantity, IntVec3 position, Map map, float radius)
        {
            Thing item = null;
            int remainingQuantity = quantity;
            while (remainingQuantity > 0)
            {
                int stackCount = 0;
                if (remainingQuantity > itemDef.stackLimit)
                {
                    stackCount = itemDef.stackLimit;
                }
                else
                {
                    stackCount = remainingQuantity;
                }
                remainingQuantity -= stackCount;
                item = ThingMaker.MakeThing(itemDef, stuff);
                item.stackCount = stackCount;
                Thing placedItem = null;
                GenDrop.TryDropSpawn(item, position + IntVec3Utility.RandomHorizontalOffset(radius), map, ThingPlaceMode.Near, out placedItem);
            }
            return item;
        }

        public virtual void Notify_PawnBoarding(Pawn pawn, bool isLastLordPawn)
        {
            this.pawnsAboard.Add(pawn);
            pawn.DeSpawn();
        }

        public bool IsTakeOffImminent(int marginTimeInTicks)
        {
            int limitTime = Math.Max(0, this.takeOffTick - marginTimeInTicks);
            if (Find.TickManager.TicksGame >= limitTime)
            {
                return true;
            }
            return false;
        }

        public ThingOwner things = null;

        public void GenerateThings()
        {
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.traderDef = this.cargoKind;
            parms.tile = this.Map.Tile;
            this.things.TryAddRangeOrTransfer(ThingSetMakerDefOf.TraderStock.root.Generate(parms), true);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.things;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public IEnumerable<FloatMenuOption> GetFloatMenuOptionsCannotReach(Pawn selPawn)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();

            FloatMenuOption noPathOption = new FloatMenuOption("CannotUseNoPath".Translate(), null);
            options.Add(noPathOption);
            return options;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            IList<Gizmo> buttonList = new List<Gizmo>();
            int groupKeyBase = 700000112;

            if (this.takeOffRequestIsEnabled)
            {
                Command_Action requestTakeOffButton = new Command_Action();
                requestTakeOffButton.icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip");
                requestTakeOffButton.defaultLabel = "Request ship take-off";
                requestTakeOffButton.defaultDesc = "Request ship take-off. Note: solar flare and other events may delay actual take-off.";
                requestTakeOffButton.activateSound = SoundDef.Named("Click");
                requestTakeOffButton.action = new Action(RequestTakeOff);
                requestTakeOffButton.groupKey = groupKeyBase + 1;
                buttonList.Add(requestTakeOffButton);
            }

            IEnumerable<Gizmo> resultButtonList;
            IEnumerable<Gizmo> basebuttonList = base.GetGizmos();
            if (basebuttonList != null)
            {
                resultButtonList = basebuttonList.Concat(buttonList);
            }
            else
            {
                resultButtonList = buttonList;
            }
            return resultButtonList;
        }
    }
}
