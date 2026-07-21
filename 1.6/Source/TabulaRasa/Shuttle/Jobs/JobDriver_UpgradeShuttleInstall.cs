using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa.Shuttle
{
    public class JobDriver_UpgradeShuttleInstall : JobDriver
    {
        public const TargetIndex target = TargetIndex.A;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve((Shuttle)job.GetTarget(target).Thing, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            Shuttle shuttle = (Shuttle)job.GetTarget(target).Thing;
            UpgradeDef upgrade = shuttle.CurrInstall;
            this.FailOnDespawnedNullOrForbidden(target);
            this.FailOn(() => shuttle == null || upgrade == null);
            if (shuttle == null || upgrade == null) { yield break; }
            if (!upgrade.costList.NullOrEmpty())
            {
                foreach(ThingDefCountClass cost in upgrade.costList)
                {
                    int remaining = cost.count;
                    Toil findResource = ToilMaker.MakeToil("FindResources");
                    findResource.initAction = delegate
                    {
                        Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(cost.thingDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t) && t.stackCount > 0);
                        if (thing == null)
                        {
                            EndJobWith(JobCondition.Incompletable);
                        }
                        else
                        {
                            job.SetTarget(TargetIndex.B, thing);
                            job.count = Mathf.Min(remaining, thing.stackCount);
                        }
                    };
                    findResource.defaultCompleteMode = ToilCompleteMode.Instant;
                    yield return findResource;
                    yield return Toils_Reserve.Reserve(TargetIndex.B);
                    yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B);
                    yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: false);
                    yield return Toils_Goto.GotoThing(target, PathEndMode.Touch);
                    Toil dropoff = ToilMaker.MakeToil("DropOffResources");
                    dropoff.initAction = delegate
                    {
                        Thing carriedThing = pawn.carryTracker.CarriedThing;
                        if (carriedThing != null)
                        {
                            int num = Mathf.Min(carriedThing.stackCount, remaining);
                            shuttle.StoreResources(carriedThing.def, carriedThing.stackCount);
                            remaining -= num;
                            if (num >= carriedThing.stackCount)
                            {
                                carriedThing.Destroy();
                            }
                            else
                            {
                                carriedThing.SplitOff(num).Destroy();
                                pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out var _);
                            }
                        }
                    };
                    dropoff.defaultCompleteMode = ToilCompleteMode.Instant;
                    yield return dropoff;
                    Toil checkLoop = ToilMaker.MakeToil("CheckRemainingResource");
                    checkLoop.initAction = delegate
                    {
                        if (remaining > 0)
                        {
                            JumpToToil(findResource);
                        }
                    };
                    checkLoop.defaultCompleteMode = ToilCompleteMode.Instant;
                    yield return checkLoop;
                }
            }
            Toil doUninstall = ToilMaker.MakeToil("DoUpgrading");
            doUninstall.tickIntervalAction = delegate (int delta)
            {
                shuttle.DoInstallWork(pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * delta);
                if (shuttle.CurrInstall == null)
                {
                    ReadyForNextToil();
                }
            };
            doUninstall.defaultCompleteMode = ToilCompleteMode.Never;
            doUninstall.WithProgressBar(target, () => (upgrade.workToInstall > 0f) ? (shuttle.installWorkRemaining / upgrade.workToInstall) : 1f);
            doUninstall.FailOnCannotTouch(target, PathEndMode.Touch);
            doUninstall.activeSkill = () => SkillDefOf.Construction;
            yield return doUninstall;
        }
    }
}
