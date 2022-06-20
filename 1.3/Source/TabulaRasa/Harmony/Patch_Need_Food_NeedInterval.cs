using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Need_Food), "NeedInterval")]
    public static class Patch_Need_Food_NeedInterval
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Need_Food __instance)
        {
            if (__instance.pawn.def.HasModExtension<DefModExt_FoodNeedAdjuster>())
            {
                DefModExt_FoodNeedAdjuster modExt = __instance.pawn.def.GetModExtension<DefModExt_FoodNeedAdjuster>();

                if (modExt.disableFoodNeed)
                {
                    __instance.CurLevel = __instance.MaxLevel;
                    if (modExt.malnutritionReplacer != null)
                    {
                        HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, -__instance.MalnutritionSeverityPerInterval);
                    }
                    else
                    {
                        HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, -__instance.MalnutritionSeverityPerInterval);
                    }
                    return false;
                }

                if (!__instance.IsFrozen)
                {
                    __instance.CurLevel -= __instance.FoodFallPerTick * 150f;
                }
                if (!__instance.Starving)
                {
                    __instance.lastNonStarvingTick = Find.TickManager.TicksGame;
                }

                if (modExt.malnutritionReplacer != null)
                {
                    if (!__instance.IsFrozen)
                    {
                        if (__instance.Starving)
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, __instance.MalnutritionSeverityPerInterval);
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, -__instance.MalnutritionSeverityPerInterval);
                        }
                    }
                }
                else
                {
                    if (!__instance.IsFrozen)
                    {
                        if (__instance.Starving)
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, __instance.MalnutritionSeverityPerInterval);
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, -__instance.MalnutritionSeverityPerInterval);
                        }
                    }
                }

                if (__instance.pawn.IsCaravanMember())
                {
                    // TODO: Fix up better, this is a really lazy way of handling it but it's fixed for short-term.
                    __instance.CurLevel = __instance.MaxLevel;
                }

                return false;
            }

            return true;
        }
    }
}
