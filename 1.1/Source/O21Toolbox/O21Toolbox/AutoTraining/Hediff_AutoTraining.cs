using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutoTraining
{
    public class Hediff_AutoTraining : HediffWithComps
    {
        [DefOf]
        public static class AutoTrainerDefOf
        {
            /// <summary>
            /// DefOfs for training options.
            /// </summary>
            public static TrainableDef Tameness;
            public static TrainableDef Obedience;
            public static TrainableDef Release;
            public static TrainableDef Rescue;
            public static TrainableDef Haul;
        }

        public int currentTick = 0;

        public override void PostMake()
        {
            base.PostMake();
            if (this.pawn.Faction.IsPlayer)
            {
                FullyTrain();
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (currentTick >= 1000 && this.pawn.Faction.IsPlayer)
            {
                FullyTrain();
                this.currentTick = 0;
            }

            this.currentTick++;
        }

        private void FullyTrain()
        {
            this.pawn.training.SetWantedRecursive(AutoTrainerDefOf.Tameness, true);
            for (int i = 0; i < AutoTrainerDefOf.Tameness.steps; i++)
            {
                this.pawn.training.Train(AutoTrainerDefOf.Tameness, null, true);
            }

            this.pawn.training.SetWantedRecursive(AutoTrainerDefOf.Obedience, true);
            for (int i = 0; i < AutoTrainerDefOf.Obedience.steps; i++)
            {
                this.pawn.training.Train(AutoTrainerDefOf.Obedience, null, true);
            }

            this.pawn.training.SetWantedRecursive(AutoTrainerDefOf.Release, true);
            for (int i = 0; i < AutoTrainerDefOf.Release.steps; i++)
            {
                this.pawn.training.Train(AutoTrainerDefOf.Release, null, true);
            }

            this.pawn.training.SetWantedRecursive(AutoTrainerDefOf.Rescue, true);
            for (int i = 0; i < AutoTrainerDefOf.Rescue.steps; i++)
            {
                this.pawn.training.Train(AutoTrainerDefOf.Rescue, null, true);
            }

            this.pawn.training.SetWantedRecursive(AutoTrainerDefOf.Haul, true);
            for (int i = 0; i < AutoTrainerDefOf.Haul.steps; i++)
            {
                this.pawn.training.Train(AutoTrainerDefOf.Haul, null, true);
            }
        }
    }
}
