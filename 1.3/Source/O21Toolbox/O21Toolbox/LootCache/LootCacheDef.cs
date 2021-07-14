using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.LootCache
{
    public class LootCacheDef : ThingDef
    {
        public IntRange rewardCount = new IntRange(1, 3);

        public float cacheWeight = 1.0f;

        public SoundDef cacheOpenSound = null;

        public List<LootCacheRewardOption> rewardOptions = new List<LootCacheRewardOption>();

        public RewardType rewardType = RewardType.Normal;
    }

    public class LootCacheRewardOption
    {
        public ThingDef def;

        public List<ThingDef> defList;

        public IntRange countRange = new IntRange(1, 1);

        public float weight = 1.0f;
    }

    public enum RewardType
    {
        Normal,
        OneOfAll,
        AllOfOne
    }
}
