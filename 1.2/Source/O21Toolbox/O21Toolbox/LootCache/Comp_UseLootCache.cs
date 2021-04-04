using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.LootCache
{
    public class Comp_UseLootCache : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            if(!(parent.def is LootCacheDef))
            {
                Log.Error("Loot Cache Comp is on a def that is not a LootCacheDef");
            }

            LootCacheDef cacheDef = (LootCacheDef)parent.def;

            if(cacheDef.cacheOpenSound != null)
            {
                cacheDef.cacheOpenSound.PlayOneShotOnCamera(usedBy.Map);
            }

            OpenCache(usedBy, cacheDef);
        }

        public void OpenCache(Pawn usedBy, LootCacheDef lootCacheDef)
        {
            IntVec3 pos = usedBy.Position;
            Map map = parent.Map;
            if (lootCacheDef.rewardOptions.NullOrEmpty())
            {
                Log.Error("Loot Cache has no rewards to give!");
                return;
            }

            int tries = Rand.RangeInclusive(lootCacheDef.rewardCount.min, lootCacheDef.rewardCount.max);
            for (int i = 0; i < tries; i++)
            {
                LootCacheRewardOption rewardOption = lootCacheDef.rewardOptions.RandomElementByWeight(new Func<LootCacheRewardOption, float>(ro => ro.weight));

                Thing thing = null;
                if (rewardOption.def != null)
                {
                    thing = ThingMaker.MakeThing(rewardOption.def, GenStuff.RandomStuffFor(rewardOption.def));
                }
                if (!rewardOption.defList.NullOrEmpty())
                {
                    ThingDef def = rewardOption.defList.RandomElement();
                    thing = ThingMaker.MakeThing(def, GenStuff.RandomStuffFor(def));
                }
                if (thing != null)
                {
                    thing.stackCount = Rand.RangeInclusive(rewardOption.countRange.min, rewardOption.countRange.max);
                    GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Direct);
                }
                else
                {
                    Log.Error("RewardOption lacking viable def or defList.");
                    return;
                }
            }
        }
    }
}
