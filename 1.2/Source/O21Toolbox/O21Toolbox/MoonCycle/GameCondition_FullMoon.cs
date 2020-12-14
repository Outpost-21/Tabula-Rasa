using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.MoonCycle
{
    public class GameCondition_FullMoon : GameCondition
    {
        private Moon moon = null;
        public Moon Moon => moon;
        private bool firstTick = true;

        private WorldComponent_MoonCycle wcMoonCycle = null;
        public WorldComponent_MoonCycle WCMoonCycle => (wcMoonCycle == null) ? wcMoonCycle = Find.World?.GetComponent<WorldComponent_MoonCycle>() : wcMoonCycle;


        public GameCondition_FullMoon() { }
        public GameCondition_FullMoon(Moon newMoon)
        {
            moon = newMoon;
        }

        public override void GameConditionTick()
        {
            //if (!O21ToolboxMod.settings.moonCycleEnabled)
            //{
            //    return;
            //}
            End();
            //base.GameConditionTick();
            //if (firstTick)
            //{
            //    firstTick = false;
            //    FullMoonTriggerHook();
            //}
        }

        /// <summary>
        /// Method specifically for postfix patching to allow other mods to trigger something when the full moon begins.
        /// </summary>
        public void FullMoonTriggerHook()
        {
            List<Pawn> allPawnsSpawned = new List<Pawn>(PawnsFinder.AllMaps);
            if ((allPawnsSpawned?.Count ?? 0) > 0)
            {
                foreach (Pawn pawn in allPawnsSpawned)
                {

                    if (pawn?.needs?.mood?.thoughts?.memories is MemoryThoughtHandler m)
                    {
                        m.TryGainMemory(MemoryDefOf.O21_SawFullMoon);
                    }
                }
            }
        }

        /// <summary>
        /// Method specifically for postfix patching to allow other mods to trigger something when the full moon ends.
        /// </summary>
        public void FullMoonEndTriggerHook()
        {

        }

        public override void End()
        {
            FullMoonEndTriggerHook();
            Messages.Message("O21_MoonCycle_FullMoonPasses".Translate(Moon.Name), MessageTypeDefOf.NeutralEvent);
            this.gameConditionManager.ActiveConditions.Remove(this);
        }

        public override string Label => "O21_MoonCycle_FullMoon".Translate(Moon.Name);
        public override string TooltipString => "O21_MoonCycle_FullMoonDesc".Translate(Moon.Name);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Moon>(ref this.moon, "moon");
            Scribe_Values.Look<bool>(ref this.firstTick, "firstTick", true);
        }
    }
}
