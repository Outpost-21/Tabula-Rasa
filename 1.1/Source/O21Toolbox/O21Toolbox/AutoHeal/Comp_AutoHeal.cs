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

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref ticksUntilNextGrow, "ticksUntilNextGrow");
            Scribe_Values.Look(ref ticksUntilNextHeal, "ticksUntilNextGrow");
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            HealUtility.SetNextHealTick(ticksUntilNextHeal, Props.healTicks);
            HealUtility.SetNextGrowTick(ticksUntilNextGrow, Props.growthTicks);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextHeal)
            {
                HealUtility.TrySealWounds(parent.pawn);
                HealUtility.SetNextHealTick(ticksUntilNextHeal, Props.healTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextGrow && Props.regrowParts)
            {
                HealUtility.TryRegrowBodyparts(parent.pawn, Props.protoBodyPart);
                HealUtility.SetNextGrowTick(ticksUntilNextGrow, Props.growthTicks);
            }
        }
    }
}
