using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class TerraformerRulesDef : Def
    {
        /// <summary>
        /// Rules for terraforming terrain tiles.
        /// </summary>
        public List<TerraformerTerrainRule> terrainRules = null;
        
        /// <summary>
        /// Rules for terraforming edifice buildings.
        /// </summary>
        public List<TerraformerThingRule> thingRules = null;
    }

    public class TerraformerTerrainRule
    {
        /// <summary>
        /// If above 0, will be used to check if fertility is above or below threshold.
        /// Both can be used. 
        /// </summary>
        public float fertilityAbove = -1f;
        public float fertilityBelow = -1f;

        /// <summary>
        /// If above 0, will use to determine a viable radius for the rule.
        /// Both can be used to determine a donut ring shape.
        /// </summary>
        public int rangeMax = -1;
        public int rangeMin = -1;

        /// <summary>
        /// If not null, will use to determine viable tiles out of what remains from other checks, if any.
        /// Use only blacklist or whitelist. Using both will result in only the whitelist being used.
        /// </summary>
        public List<TerrainDef> terrainDefsWhitelist = null;
        public List<TerrainDef> terrainDefsBlacklist = null;

        /// <summary>
        /// The resulting terrain a viable tile will be turned into.
        /// </summary>
        public TerrainDef terrainResult = null;
    }

    public class TerraformerThingRule
    {
        /// <summary>
        /// If above 0, will use to determine a viable radius for the rule.
        /// Both can be used to determine a donut ring shape.
        /// </summary>
        public int rangeMax = -1;
        public int rangeMin = -1;

        /// <summary>
        /// If not null, will use to determine viable tiles out of what remains from other checks, if any.
        /// Use only blacklist or whitelist. Using both will result in only the whitelist being used.
        /// </summary>
        public List<ThingDef> thingDefsWhitelist = null;
        public List<ThingDef> thingDefsBlacklist = null;

        /// <summary>
        /// The resulting thingdef a viable thing will be turned into.
        /// </summary>
        public ThingDef thingResult = null;
    }
}
