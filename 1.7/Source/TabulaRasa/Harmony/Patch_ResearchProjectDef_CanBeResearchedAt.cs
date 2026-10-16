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
        public static void Postfix(ResearchProjectDef __instance, ref bool __result, Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
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

                    // If techLevel is set, limit research based on that.
                    // Only attempt if the project is normally researchable currently.
                    //if(__result && modExt.techLevel != TechLevel.Undefined)
                    //{
                    //    __result = modExt.techLevel == __instance.techLevel;
                    //}
                }
            }
        }
    }
}
