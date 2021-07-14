using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.AutoHeal
{
    public class Comp_AutoHeal : HediffComp
    {
        public CompProperties_AutoHeal Props => (CompProperties_AutoHeal)props;
        public int ticksUntilNextHeal;
        public int ticksUntilNextGrow;
        public int ticksUntilNextCure;

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

            HealUtility.SetNextTick(ticksUntilNextHeal, Props.healTicks);
            HealUtility.SetNextTick(ticksUntilNextGrow, Props.growthTicks);
            HealUtility.SetNextTick(ticksUntilNextCure, Props.cureTicks);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextHeal)
            {
                HealUtility.TrySealWounds(parent.pawn, Props.ignoreWhenHealing);
                HealUtility.SetNextTick(ticksUntilNextHeal, Props.healTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextGrow && Props.regrowParts)
            {
                HealUtility.TryRegrowBodyparts(parent.pawn, Props.protoBodyPart);
                HealUtility.SetNextTick(ticksUntilNextGrow, Props.growthTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextCure && Props.removeInfections)
            {
                HealUtility.TryCureInfections(parent.pawn, Props.infectionsAllowed, Props.explicitRemovals);
                HealUtility.SetNextTick(ticksUntilNextCure, Props.cureTicks);
            }
        }
    }
}
