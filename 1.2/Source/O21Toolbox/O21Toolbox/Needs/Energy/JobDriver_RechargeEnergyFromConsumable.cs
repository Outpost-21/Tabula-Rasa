﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using O21Toolbox.HarmonyPatches;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Grabs a consumable energy source and recharge themselves or their target Pawn with it.
    /// </summary>
    public class JobDriver_RechargeEnergyFromConsumable : JobDriver
    {
        private const TargetIndex PowerDestIndex = TargetIndex.A;
        private const TargetIndex OtherPawnIndex = TargetIndex.B;

        public Need_Energy energyNeed;

        public override void Notify_Starting()
        {
            base.Notify_Starting();

            energyNeed = GetActor().needs.TryGetNeed<Need_Energy>();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.CanReserve(TargetA))
                return false;

            if(TargetB.IsValid)
            {
                if (!pawn.CanReserve(TargetB))
                {
                    return false;
                }
                else
                {
                    pawn.Reserve(TargetB, job, errorOnFailed: errorOnFailed);
                }
            }

            pawn.Reserve(TargetA, job, errorOnFailed: errorOnFailed);

            return true;
        }

        public override string GetReport()
        {
            if (TargetB.IsValid)
            {
                return ReportStringProcessed(job.def.GetModExtension<ExtraReportStringProperties>().extraReportString);
            }
            else
            {
                return base.GetReport();
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(PowerDestIndex);
            if (!TargetB.IsValid)
                AddFailCondition(() => energyNeed == null);

            yield return Toils_Reserve.Reserve(PowerDestIndex);
            if (TargetB.IsValid)
                yield return Toils_Reserve.Reserve(OtherPawnIndex);

            yield return Toils_Goto.GotoThing(PowerDestIndex, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting(PowerDestIndex);
            yield return Toils_Reserve.Release(PowerDestIndex);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, true, false);

            if(TargetB.IsValid)
            {
                //Recharge someone else.
                yield return Toils_Goto.GotoThing(OtherPawnIndex, PathEndMode.Touch).FailOnForbidden(OtherPawnIndex);

                yield return Toils_General.Wait(100).WithProgressBarToilDelay(TargetIndex.A, false);

                Toil rechargeToil = new Toil();
                rechargeToil.AddFinishAction(delegate ()
                {
                    //Use up the carried stack 
                    Thing carriedThing = pawn.carryTracker.CarriedThing;
                    if (carriedThing != null)
                    {
                        Comp_EnergySource energyComp = carriedThing.TryGetComp<Comp_EnergySource>();
                        if (energyComp != null)
                            energyComp.RechargeEnergyNeed((Pawn)TargetB.Thing);

                        pawn.carryTracker.DestroyCarriedThing();
                    }
                });

                yield return rechargeToil;
                yield return Toils_Reserve.Release(OtherPawnIndex);
            }
            else
            {
                yield return Toils_General.Wait(100).WithProgressBarToilDelay(TargetIndex.A, false);

                //Recharge user.
                Toil rechargeToil = new Toil();
                rechargeToil.AddFinishAction(delegate ()
                {
                    //Use up the carried stack 
                    Thing carriedThing = pawn.carryTracker.CarriedThing;
                    if(carriedThing != null)
                    {
                        Comp_EnergySource energyComp = carriedThing.TryGetComp<Comp_EnergySource>();
                        if (energyComp != null)
                            energyComp.RechargeEnergyNeed(pawn);
                        
                        if (!carriedThing.def.HasModExtension<DefModExt_OutputFromEdible>())
                        {
                            pawn.carryTracker.DestroyCarriedThing();
                        }
                        else //Drop output items if they exist.
                        {
                            DefModExt_OutputFromEdible ext = carriedThing.def.GetModExtension<DefModExt_OutputFromEdible>();
                            if (ext.outputThing != null)
                            {
                                Thing output = ThingMaker.MakeThing(ext.outputThing);
                                output.stackCount = pawn.carryTracker.CarriedThing.stackCount;

                                pawn.carryTracker.DestroyCarriedThing();

                                //if (pawn.IsColonist)
                                //{
                                //    pawn.inventory.innerContainer.TryAddOrTransfer(output);
                                //}
                                //else
                                //{
                                    if(!GenPlace.TryPlaceThing(output, pawn.Position, pawn.Map, ThingPlaceMode.Near))
                                    {
                                        Log.Error(string.Concat(new object[]
                                        {
                                            pawn,
                                            " could not drop recipe product ",
                                            output,
                                            " near ",
                                            pawn.Position
                                        }), false);
                                    }
                                //}
                            }
                        }
                    }
                });

                yield return rechargeToil;
            }
        }
    }
}