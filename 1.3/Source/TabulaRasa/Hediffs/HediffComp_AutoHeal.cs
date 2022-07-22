using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class HediffComp_AutoHeal : HediffComp
    {
        public int ticksUntilNextHeal;
        public int ticksUntilNextGrow;
        public int ticksUntilNextCure;

        public HediffCompProperties_AutoHeal Props => (HediffCompProperties_AutoHeal)props;

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref ticksUntilNextGrow, "ticksUntilNextGrow");
            Scribe_Values.Look(ref ticksUntilNextHeal, "ticksUntilNextHeal");
            Scribe_Values.Look(ref ticksUntilNextCure, "ticksUntilNextCure");
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            HealthUtil.SetNextTick(ticksUntilNextHeal, Props.healTicks);
            HealthUtil.SetNextTick(ticksUntilNextGrow, Props.growthTicks);
            HealthUtil.SetNextTick(ticksUntilNextCure, Props.cureTicks);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextHeal)
            {
                HealthUtil.TrySealWounds(parent.pawn, Props.ignoreWhenHealing);
                HealthUtil.SetNextTick(ticksUntilNextHeal, Props.healTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextGrow && Props.regrowParts)
            {
                HealthUtil.TryRegrowBodyparts(parent.pawn, Props.protoBodyPart);
                HealthUtil.SetNextTick(ticksUntilNextGrow, Props.growthTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextCure && Props.removeInfections)
            {
                HealthUtil.TryCureInfections(parent.pawn, Props.infectionsAllowed, Props.explicitRemovals);
                HealthUtil.SetNextTick(ticksUntilNextCure, Props.cureTicks);
            }
        }
    }
}
