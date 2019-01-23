using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace O21Toolbox.Bunker
{
    public class JobDriver_EnterEmplacement : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil enter = new Toil();
            enter.initAction = delegate ()
            {
                Pawn actor = enter.actor;
                Building_BunkerEmplacement building = (Building_BunkerEmplacement)actor.CurJob.targetA.Thing;
                Action action = delegate ()
                {
                    bool flag = building.GetInner().InnerListForReading.Count >= building.maxCount;
                    if (!flag)
                    {
                        actor.DeSpawn(DestroyMode.Vanish);
                        building.TryAcceptThing(actor, true);
                    }
                };
                action();
            };
            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            enter.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            yield return enter;
            yield break;
        }
    }
}
