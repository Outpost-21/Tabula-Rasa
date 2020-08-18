using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Teleporter
{
    public class JobDriver_UseTeleporter : JobDriver
    {
        private int useDuration = -1;

        private Thing destination;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.useDuration, "useDuration", 0, false);
            Scribe_References.Look(ref destination, "destination");
        }
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            Comp_Teleporter teleporter = this.job.GetTarget(TargetIndex.A).Thing.TryGetComp<Comp_Teleporter>();
            useDuration = teleporter.Props.useDuration;
            destination = teleporter.target;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Don't want this to actually reserve it, multiple people should be able to use them at once.
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil toil = Toils_General.Wait(useDuration, TargetIndex.None);
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return toil;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                Thing sendTeleporter = actor.CurJob.targetA.Thing;
                Action action = delegate ()
                {
                    if(destination != null)
                    {
                        sendTeleporter.TryGetComp<Comp_Teleporter>().TeleportEffect(actor);
                        actor.DeSpawn();
                        GenSpawn.Spawn(actor, destination.Position, destination.Map);
                        destination.TryGetComp<Comp_Teleporter>().TeleportEffect(actor);
                    }
                    else
                    {
                        Messages.Message("Teleport destination no longer valid.", MessageTypeDefOf.CautionInput);
                    }
                };
                action();
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }
    }
}
