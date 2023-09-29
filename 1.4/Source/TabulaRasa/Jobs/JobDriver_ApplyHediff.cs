using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa
{
    public class JobDriver_ApplyHediff : JobDriver
    {
        public int useDuration = -1;

        public Thing destination;

        private const int DurationTicks = 600;

        public HediffDef hediffDef;
        private Thing Item
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.useDuration, "useDuration", 0, false);
            Scribe_References.Look(ref destination, "destination");
            Scribe_Defs.Look(ref hediffDef, "hediffDef");
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(job.GetTarget(TargetIndex.A).Thing as Corpse != null ? job.GetTarget(TargetIndex.A).Thing as Corpse : job.GetTarget(TargetIndex.A).Thing as Pawn, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Item, this.job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(600, TargetIndex.None);
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return toil;
            yield return Toils_General.Do(new Action(ApplyHediff));
            yield break;
        }

        public void ApplyHediff()
        {
            Pawn pawn = job.GetTarget(TargetIndex.A).Thing as Pawn;
            pawn.health.AddHediff(hediffDef);
            Item.SplitOff(1).Destroy(DestroyMode.Vanish);
        }
    }
}
