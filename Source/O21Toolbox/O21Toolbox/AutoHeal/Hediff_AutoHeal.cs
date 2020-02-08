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

        public override void PostMake()
        {
            base.PostMake();
            DefModExtension_AutoHealProps defExt = def.TryGetModExtension<DefModExtension_AutoHealProps>();
            HealUtility.SetNextHealTick(ticksUntilNextHeal, defExt.healTicks);
            HealUtility.SetNextGrowTick(ticksUntilNextGrow, defExt.growthTicks);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksUntilNextHeal, "ticksUntilNextHeal");
            Scribe_Values.Look(ref ticksUntilNextGrow, "ticksUntilNextGrow");
        }

        public override void Tick()
        {
            base.Tick();

            DefModExtension_AutoHealProps defExt = def.TryGetModExtension<DefModExtension_AutoHealProps>();
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextHeal)
            {
                HealUtility.TrySealWounds(pawn);
                HealUtility.SetNextHealTick(ticksUntilNextHeal, defExt.healTicks);
            }
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextGrow && defExt.regrowParts)
            {
                HealUtility.TryRegrowBodyparts(pawn, defExt.protoBodyPart);
                HealUtility.SetNextGrowTick(ticksUntilNextGrow, defExt.growthTicks);
            }
        }
    }
}
