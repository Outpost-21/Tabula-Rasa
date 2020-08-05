using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Research;
using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(ResearchProjectDef), "PlayerHasAnyAppropriateResearchBench", MethodType.Getter)]
    public static class Patch_PlayerHasAnyAppropriateResearchBench_Postfix
    {
        public static void PostFix(ResearchProjectDef __instance, ref bool __result)
        {
            if (!__result)
            {
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    List<Building> allBuildingsColonist = maps[i].listerBuildings.allBuildingsColonist;
                    for (int j = 0; j < allBuildingsColonist.Count; j++)
                    {
                        DefModExt_ResearchBenchSubstitutes comp = allBuildingsColonist[j].def.TryGetModExtension<DefModExt_ResearchBenchSubstitutes>();
                        if (comp != null)
                        {
                            if (__instance.requiredResearchBuilding != null && comp.actLikeResearchBench.Contains(__instance.requiredResearchBuilding))
                            {
                                __result = true;
                            }

                            if (!__instance.requiredResearchFacilities.NullOrEmpty<ThingDef>())
                            {
                                bool hasFacilities = true;
                                foreach (ThingDef facility in __instance.requiredResearchFacilities)
                                {
                                    if (!comp.actLikeResearchFacility.Contains(facility))
                                    {
                                        hasFacilities = false;
                                    }
                                }
                                __result = hasFacilities;
                            }
                        }
                    }
                }
            }
        }
    }
}
