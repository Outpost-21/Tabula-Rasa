using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.AutoHeal
{
    public class Hediff_AutoHeal : HediffWithComps
    {
        public int ticksUntilNextHeal;
        public int ticksUntilNextGrow;
        public int ticksUntilNextCure;


        DefModExtension_AutoHealProps DefExt => def.TryGetModExtension<DefModExtension_AutoHealProps>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksUntilNextHeal, "ticksUntilNextHeal");
            Scribe_Values.Look(ref ticksUntilNextGrow, "ticksUntilNextGrow");
            Scribe_Values.Look(ref ticksUntilNextCure, "ticksUntilNextCure");
        }

        public override void PostMake()
        {
            base.PostMake();
            HealUtility.SetNextTick(ticksUntilNextHeal, DefExt.healTicks);
            HealUtility.SetNextTick(ticksUntilNextGrow, DefExt.growthTicks);
            HealUtility.SetNextTick(ticksUntilNextCure, DefExt.cureTicks);
        }

        public override void Tick()
        {
            base.Tick();
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextHeal)
            {
                HealUtility.TrySealWounds(pawn, DefExt.ignoreWhenHealing);
                HealUtility.SetNextTick(ticksUntilNextHeal, DefExt.healTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextGrow && DefExt.regrowParts)
            {
                HealUtility.TryRegrowBodyparts(pawn, DefExt.protoBodyPart);
                HealUtility.SetNextTick(ticksUntilNextGrow, DefExt.growthTicks);
            }
            if (Current.Game.tickManager.TicksGame >= ticksUntilNextCure && DefExt.removeInfections)
            {
                HealUtility.TryCureInfections(pawn, DefExt.infectionsAllowed, DefExt.explicitRemovals);
                HealUtility.SetNextTick(ticksUntilNextCure, DefExt.cureTicks);
            }
        }
    }
}
