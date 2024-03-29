﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Gene_Regeneration : Gene
    {
        public int ticksUntilNextHeal = -1;
        public int ticksUntilNextGrow = -1;
        public int ticksUntilNextCure = -1;

        public DefModExt_GeneRegeneration modExt;

        public DefModExt_GeneRegeneration ModExt
        {
            get
            {
                if(modExt == null)
                {
                    modExt = def.GetModExtension<DefModExt_GeneRegeneration>();
                }
                return modExt;
            }
        }

        public override void Tick()
        {
            base.Tick();
            if(ModExt != null)
            {
                if (Current.Game.tickManager.TicksGame >= ticksUntilNextHeal)
                {
                    HealthUtil.TrySealWounds(pawn, ModExt.ignoreWhenHealing);
                    HealthUtil.SetNextTick(ticksUntilNextHeal, ModExt.healTicks);
                }
                if (Current.Game.tickManager.TicksGame >= ticksUntilNextGrow && ModExt.regrowParts)
                {
                    HealthUtil.TryRegrowBodyparts(pawn, ModExt.protoBodyPart);
                    HealthUtil.SetNextTick(ticksUntilNextGrow, ModExt.growthTicks);
                }
                if (Current.Game.tickManager.TicksGame >= ticksUntilNextCure && ModExt.removeInfections)
                {
                    HealthUtil.TryCureInfections(pawn, ModExt.infectionsAllowed, ModExt.explicitRemovals);
                    HealthUtil.SetNextTick(ticksUntilNextCure, ModExt.cureTicks);
                }
            }
            else
            {
                LogUtil.Warning($"GeneDef {def.defName} has a null DefModExt_GeneRegeneration so regeneration will not function properly.");
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticksUntilNextGrow, "ticksUntilNextGrow");
            Scribe_Values.Look(ref ticksUntilNextHeal, "ticksUntilNextHeal");
            Scribe_Values.Look(ref ticksUntilNextCure, "ticksUntilNextCure");
        }
    }
}
