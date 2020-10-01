using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.MoonCycle
{
    public class GameCondition_MoonCycle : GameCondition
    {
        private WorldComponent_MoonCycle wcMoonCycle = null;
        public WorldComponent_MoonCycle WCMoonCycle
        {
            get
            {
                if (wcMoonCycle == null)
                {
                    wcMoonCycle = Find.World?.GetComponent<WorldComponent_MoonCycle>();
                }
                return wcMoonCycle;
            }
        }

        public int SoonestFullMoonInDays
        {
            get
            {
                int result = -1;
                if (this.WCMoonCycle.moons is List<Moon> moons && !moons.NullOrEmpty())
                {
                    for (int i = 0; i < moons.Count; i++)
                    {
                        if (result == -1) result = moons[i].DaysUntilFull;
                        if (moons[i].DaysUntilFull < result) result = moons[i].DaysUntilFull;
                    }
                }
                return result;
            }
        }

        public override string Label
        {
            get
            {
                string result = "";
                if (SoonestFullMoonInDays > 0)
                {
                    result = "O21_MoonCycle_UntilNextFullMoon".Translate(SoonestFullMoonInDays);
                }
                else
                {
                    result = "O21_MoonCycle_FullMoonImminentArgless".Translate();
                }
                return result;
            }
        }

        public override void End()
        {
            this.gameConditionManager.ActiveConditions.Remove(this);
        }

        public override string TooltipString
        {
            get
            {
                string result = "";
                result = "O21_MoonCycle_CurrentPhaseDesc".Translate(WCMoonCycle.world.info.name);
                StringBuilder s = new StringBuilder();
                s.AppendLine(result);
                s.AppendLine();
                if (WCMoonCycle.moons is List<Moon> MoonList && !MoonList.NullOrEmpty())
                {
                    s.AppendLine("O21_MoonCycle_Moons".Translate(WCMoonCycle.world.info.name));
                    s.AppendLine("------");

                    foreach (Moon m in MoonList)
                    {
                        int daysLeft = m.DaysUntilFull;
                        if (daysLeft > 0) s.AppendLine("  " + "O21_MoonCycle_CurrentPhase".Translate(m.Name, m.DaysUntilFull));
                        else s.AppendLine("  " + "O21_MoonCycle_FullMoonImminent".Translate(m.Name));
                    }
                }
                return s.ToString().TrimEndNewlines();
            }
        }
    }
}
