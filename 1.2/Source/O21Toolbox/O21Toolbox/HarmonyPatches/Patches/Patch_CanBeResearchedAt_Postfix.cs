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
    [HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
    public static class Patch_CanBeResearchedAt_Postfix
    {
        [HarmonyPostfix]
        public static void PostFix(ResearchProjectDef __instance, ref bool __result, Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
        {
            if (!__result)
            {
                // Double check power, result could be false because of that.
                if (!ignoreResearchBenchPowerStatus)
                {
                    CompPowerTrader power = bench.GetComp<CompPowerTrader>();
                    if (power != null && !power.PowerOn)
                    {
                        return;
                    }
                }
                // Try and get modExt
                DefModExt_ResearchBenchSubstitutes modExt = bench.def.TryGetModExtension<DefModExt_ResearchBenchSubstitutes>();
                if (modExt != null)
                {
                    // If modExt exists, check if it's a viable bench based on the info provided.
                    if (modExt.actLikeResearchBench.Contains(__instance.requiredResearchBuilding))
                    {
                        __result = true;
                    }

                    if (!__instance.requiredResearchFacilities.NullOrEmpty<ThingDef>())
                    {
                        bool hasFacilities = true;
                        foreach (ThingDef facility in __instance.requiredResearchFacilities)
                        {
                            if (!modExt.actLikeResearchFacility.Contains(facility))
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
