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
                        LogUtil.LogError("Comp_Mining: cachedMineableThings list is empty, this is something that should never happen, vanilla contains many of these items.");
                    }

                    return cachedMineableThings = enumerable.ToList();
                }
                return cachedMineableThings;
            }
        }
    }
}
