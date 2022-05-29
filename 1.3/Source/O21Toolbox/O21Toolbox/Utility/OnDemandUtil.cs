using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox
{
    public static class OnDemandUtil
    {
        public static void FixOnDemandDefs(string prefix, ModContentPack mcp)
        {
            // Things
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Incidents
            foreach (IncidentDef def in DefDatabase<IncidentDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Quests
            foreach (QuestScriptDef def in DefDatabase<QuestScriptDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Game Conditions
            foreach (GameConditionDef def in DefDatabase<GameConditionDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Weathers
            foreach (WeatherDef def in DefDatabase<WeatherDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Factions
            foreach (FactionDef def in DefDatabase<FactionDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // PawnKinds
            foreach (PawnKindDef def in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Works
            foreach (WorkGiverDef def in DefDatabase<WorkGiverDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Floors
            foreach (TerrainDef def in DefDatabase<TerrainDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Recipes
            foreach (RecipeDef def in DefDatabase<RecipeDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Research Projects
            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Precepts
            foreach (PreceptDef def in DefDatabase<PreceptDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Gatherings
            foreach (GatheringDef def in DefDatabase<GatheringDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Interactions
            foreach (InteractionDef def in DefDatabase<InteractionDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // JoyGivers
            foreach (JoyGiverDef def in DefDatabase<JoyGiverDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Thoughts
            foreach (ThoughtDef def in DefDatabase<ThoughtDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Traits
            foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Abilities
            foreach (AbilityDef def in DefDatabase<AbilityDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
            // Hediffs
            foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs)
            {
                if (def.defName.Contains(prefix) && def.fileName.NullOrEmpty())
                {
                    FixMissingMCPData(def, mcp);
                }
            }
        }

        public static void FixMissingMCPData(Def def, ModContentPack mcp)
        {
            if (mcp.AllDefs.Contains(def))
            {
                LogUtil.LogWarning($"{mcp.Name} already contains def: {def.defName}");
                def.modContentPack = mcp;
                def.fileName = mcp.Name;
                return;
            }
            mcp.AddDef(def, mcp.Name);
        }
    }
}
