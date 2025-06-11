using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TabulaRasa
{
    public class JobDriver_GatherSlotItem : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        public override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1);
            var toil = new Toil
            {
                initAction = delegate { pawn.pather.StartPath(TargetThingA, PathEndMode.ClosestTouch); },
                defaultCompleteMode = ToilCompleteMode.PatherArrival
            };
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return toil;
            yield return new Toil
            {
                initAction = delegate
                {
                    var itemToGather = job.targetA.Thing;
                    Thing itemToGatherSplit;
                    if (itemToGather.def.stackLimit > 1 && itemToGather.stackCount > 1)
                    {
                        itemToGatherSplit = itemToGather.SplitOff(1);
                    }
                    else
                    {
                        itemToGatherSplit = itemToGather;
                    }

                    var pawn_EquipmentTracker = pawn.equipment;
                    if (pawn_EquipmentTracker != null)
                    {
                        var thingWithComps = pawn_EquipmentTracker.Primary;

                        if (thingWithComps != null)
                        {
                            var CompSlotLoadable = thingWithComps.GetComp<Comp_SlotLoadable>();
                            if (CompSlotLoadable != null)
                            {
                                CompSlotLoadable.TryLoadSlot(itemToGather);
                                if (thingWithComps.def.soundInteract != null)
                                {
                                    thingWithComps.def.soundInteract.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                                }
                            }
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
