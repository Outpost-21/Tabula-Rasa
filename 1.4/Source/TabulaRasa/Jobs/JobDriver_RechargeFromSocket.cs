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
    //TODO: Remove next RW update
    public class JobDriver_RechargeFromSocket : JobDriver
    {
        private const TargetIndex PowerDestIndex = TargetIndex.A;

        private const TargetIndex AlternateDestIndex = TargetIndex.B;

        public Building powerBuilding => TargetA.Thing as Building;

        public Need_Energy energyNeed;

        public int ticksSpentCharging = 0;

        public const int maxTicksSpentCharging = 300;

        public float powerNetEnergyDrainedPerTick = 1.32f;

        public override void Notify_Starting()
        {
            base.Notify_Starting();

            energyNeed = GetActor().needs.TryGetNeed<Need_Energy>();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticksSpentCharging, "ticksSpentCharging");
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(PowerDestIndex);
            AddFailCondition(() => energyNeed == null);

            yield return Toils_Reserve.Reserve(PowerDestIndex);
            if (!TargetB.IsValid)
            {
                yield return Toils_Goto.GotoThing(PowerDestIndex, PathEndMode.ClosestTouch);
            }
            else
            {
                yield return Toils_Reserve.Reserve(AlternateDestIndex);
                yield return Toils_Goto.GotoThing(AlternateDestIndex, PathEndMode.OnCell);
            }

            Toil rechargeToil = new Toil();

            rechargeToil.tickAction = delegate
            {
                CompPowerBattery compBattery = powerBuilding.PowerComp?.PowerNet?.batteryComps?.FirstOrDefault(battery => battery.StoredEnergy > powerNetEnergyDrainedPerTick);

                if (compBattery != null)
                {
                    compBattery.DrawPower(powerNetEnergyDrainedPerTick);

                    energyNeed.CurLevel += energyNeed.MaxLevel / maxTicksSpentCharging;
                }

                ticksSpentCharging++;
            };
            rechargeToil.AddEndCondition(delegate
            {
                if (powerBuilding == null || powerBuilding.PowerComp == null)
                {
                    return JobCondition.Incompletable;
                }

                if (powerBuilding.PowerComp?.PowerNet.CurrentStoredEnergy() < powerNetEnergyDrainedPerTick)
                {
                    return JobCondition.Incompletable;
                }

                if (energyNeed.CurLevelPercentage >= 0.99f)
                {
                    return JobCondition.Succeeded;
                }

                if (ticksSpentCharging > maxTicksSpentCharging)
                {
                    return JobCondition.Incompletable;
                }

                return JobCondition.Ongoing;
            });

            if (!TargetB.IsValid)
            {
                rechargeToil.FailOnCannotTouch(PowerDestIndex, PathEndMode.ClosestTouch);
            }
            else
            {
                rechargeToil.FailOnCannotTouch(AlternateDestIndex, PathEndMode.OnCell);
            }

            rechargeToil.WithProgressBar(TargetIndex.A, () => energyNeed.CurLevelPercentage, false);
            rechargeToil.defaultCompleteMode = ToilCompleteMode.Never;
            yield return rechargeToil;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.CanReserve(TargetA))
            {
                return false;
            }

            if (TargetB.IsValid)
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
    }
}
