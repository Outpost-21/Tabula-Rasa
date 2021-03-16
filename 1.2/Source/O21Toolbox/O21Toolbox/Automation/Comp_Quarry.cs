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
    public class Comp_Quarry : ThingComp
    {
        public CompProperties_Quarry Props => (CompProperties_Quarry)props;

        public CompFlickable compFlickable;
        public CompPowerTrader compPower;
        public CompRefuelable compRefuelable;

        public List<ThingDef> cachedMineableThings = new List<ThingDef>();
        public Dictionary<ThingDef, bool> mineableThings = new Dictionary<ThingDef, bool>();

        public List<ThingDef> CachedMineableThings
        {
            get
            {
                if(cachedMineableThings.NullOrEmpty())
                {
                    IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                                  where (def.building != null && def.building.isNaturalRock && def.building.mineableThing != null) || def.HasModExtension<DefModExt_QuarryThing>()
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
        protected Dictionary<ThingDef, bool> MineableThings
        {
            get
            {
                if (mineableThings.EnumerableNullOrEmpty())
                {
                    foreach(ThingDef def in CachedMineableThings)
                    {
                        if (def.HasModExtension<DefModExt_QuarryThing>())
                        {
                            mineableThings.Add(def, true);
                        }
                        else
                        {
                            mineableThings.Add(def.building.mineableThing, true);
                        }
                    }
                }
                return mineableThings;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            compFlickable = parent.GetComp<CompFlickable>();
            compPower = parent.GetComp<CompPowerTrader>();
            compRefuelable = parent.GetComp<CompRefuelable>();
        }


    }
}
