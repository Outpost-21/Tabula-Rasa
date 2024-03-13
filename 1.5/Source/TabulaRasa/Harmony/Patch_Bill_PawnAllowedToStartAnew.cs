using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Bill), "PawnAllowedToStartAnew")]
    public static class Patch_Bill_PawnAllowedToStartAnew
    {
        [HarmonyPostfix]
        public static void PostFix(ref Bill __instance, Pawn p, ref bool __result)
        {
            if(__result == true)
            {
                DefModExt_RecipeExtender modExt = __instance.recipe.GetModExtension<DefModExt_RecipeExtender>();
                if (modExt != null)
                {
                    if (modExt.requiredHediff != null && !p.health.hediffSet.HasHediff(modExt.requiredHediff))
                    {
                        JobFailReason.Is(modExt.requiredHediffMissingMsg.Translate());
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
}
