using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
    public static class Patch_ResearchProjectDef_CanBeResearchedAt
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
                DefModExt_ResearchBenchSubstitutes modExt = bench.def.GetModExtension<DefModExt_ResearchBenchSubstitutes>();
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
