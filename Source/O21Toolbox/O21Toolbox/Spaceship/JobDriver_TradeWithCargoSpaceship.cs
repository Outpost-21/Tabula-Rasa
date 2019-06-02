using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace O21Toolbox.Spaceship
{
    public class JobDriver_TradeWithCargoSpaceship : JobDriver
    {
        public TargetIndex cargoSpaceshipIndex = TargetIndex.A;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Spaceship_Building cargoSpaceship = this.TargetThingA as Spaceship_Building;

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOn(delegate ()
            {
                return (cargoSpaceship.DestroyedOrNull()
                    || (cargoSpaceship.CanTradeNow == false));
            });

            Toil faceSpaceshipToil = new Toil()
            {
                initAction = () =>
                {
                    this.GetActor().rotationTracker.FaceCell(cargoSpaceship.Position);
                }
            };
            yield return faceSpaceshipToil;

            Toil tradeWithSpacehipToil = new Toil()
            {
                initAction = () =>
                {
                    Find.WindowStack.Add(new Dialog_Trade(this.GetActor(), cargoSpaceship as ITrader));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return tradeWithSpacehipToil;

            yield return Toils_Reserve.Release(cargoSpaceshipIndex);
        }
    }
}
