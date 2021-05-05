using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Automation
{
    [StaticConstructorOnStartup]
    public class MiningUtility
    {
        public static List<ThingDef> cachedMineableThings = new List<ThingDef>();

        public static List<ThingDef> CachedMineableThings
        {
            get
            {
                if (cachedMineableThings.NullOrEmpty())
                {
                    IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                                       where def.deepCommonality > 0
                                                       select def;

                    if (enumerable.EnumerableNullOrEmpty())
                    {
                        Log.Error("O21Toolbox.Automation.Comp_Quarry: cachedMineableThings list is empty, this is something that should never happen, vanilla contains many of these items.");
                    }

                    return cachedMineableThings = enumerable.ToList();
                }
                return cachedMineableThings;
            }
        }
    }
}
