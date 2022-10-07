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
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    public static class Patch_Pawn_NeedsTracker_ShouldHaveNeed
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
        {
            if (nd == TabulaRasaDefOf.TabulaRasa_EnergyNeed)
            {
                __result = __instance.pawn.def.HasModExtension<DefModExt_EnergyNeed>();
            }
        }
    }
}
