using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace TabulaRasa.Shuttle
{
    public class JobDriver_LoadShuttleShells : JobDriver
    {
        private const TargetIndex BuildingInd = TargetIndex.A;

        private const TargetIndex ComponentInd = TargetIndex.B;

        private const int TicksDuration = 1000;

        private Shuttle Building => (Shuttle)job.GetTarget(TargetIndex.A).Thing;

        private Thing Components => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Building, job, 1, -1, null, errorOnFailed) && pawn.Reserve(Components, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            Pawn Pawn = pawn;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(100);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.activeSkill = () => SkillDefOf.Construction;
            yield return toil;
            Toil toil2 = ToilMaker.MakeToil("MakeNewToils");
            toil2.initAction = delegate
            {
                Comp_ShellLoadable comp_CanLoadShell = Building.TryGetComp<Comp_ShellLoadable>();
                int num = 0;
                for (int i = 0; i < comp_CanLoadShell.ListShell.Count; i++)
                {
                    num += comp_CanLoadShell.ListShell[i].stackCount;
                }
                if (comp_CanLoadShell.ListShell.Count < comp_CanLoadShell.Props.LoadShell_Max)
                {
                    comp_CanLoadShell.DoSomething_CarryShell(Components);
                    Components.Destroy();
                    if (num < comp_CanLoadShell.Props.LoadShell_Max)
                    {
                        Thing thing = FindClosestComponent(Pawn);
                        Job job = JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_LoadShuttleShell, Building, thing);
                        job.count = 1;
                        Pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }
                    else if (num >= comp_CanLoadShell.Props.LoadShell_Max)
                    {
                        Pawn.records.Increment(RecordDefOf.ThingsRepaired);
                        Pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                    }
                }
                else if (num >= comp_CanLoadShell.Props.LoadShell_Max)
                {
                    Pawn.records.Increment(RecordDefOf.ThingsRepaired);
                    Pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
            };
            yield return toil2;
        }

        private Thing FindClosestComponent(Pawn pawn)
        {
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Shell), PathEndMode.OnCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false, false, false), 9999f, (Predicate<Thing>)((Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x)), (IEnumerable<Thing>)null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}
