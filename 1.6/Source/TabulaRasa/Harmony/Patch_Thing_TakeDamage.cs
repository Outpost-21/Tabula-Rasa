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
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_Thing_TakeDamage
    {
        [HarmonyPrefix]
        public static bool Prefix(DamageInfo dinfo, ref Thing __instance, ref DamageWorker.DamageResult __result)
        {
            if (__instance is Pawn pawn)
            {
                if (pawn.def.HasModExtension<DefModExt_Fireproof>() && dinfo.Def == DamageDefOf.Flame)
                {
                    __result = new DamageWorker.DamageResult();
                    return false;
                }
            }

            return true;
        }
    }
}
