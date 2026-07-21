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
    public abstract class JobDriver_EnterShuttleBase : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil toil = Toils_General.Wait(60);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            yield return toil;
            Toil enter = new Toil();

            enter.initAction = delegate
            {
                Shuttle shuttle = (Shuttle)pawn.CurJob.targetA.Thing;
                Action action = delegate
                {
                    if (shuttle.IsLanded)
                    {
                        if (!pawn.RaceProps.Animal)
					{
                            BoardShuttle(shuttle, pawn);
                        }
                    }
                    else if (!shuttle.IsLanded)
                    {
                        string text = "TabulaRasa.CannotEnterShuttleInFlight".Translate();
                        MoteMaker.ThrowText(shuttle.DrawPos, shuttle.Map, text, 2.5f);
                    }
                };
                action();
            };
            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;
        }

        public virtual void BoardShuttle(Shuttle shuttle, Pawn pawn)
        {

        }
    }
}
