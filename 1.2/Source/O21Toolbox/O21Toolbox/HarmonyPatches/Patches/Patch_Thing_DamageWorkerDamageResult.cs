using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.PawnExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_Fireproof_Thing_TakeDamage
    {
        [HarmonyPrefix]
        public static bool Prefix(DamageInfo dinfo, ref Thing __instance, ref DamageWorker.DamageResult __result)
        {
            if(__instance is Pawn pawn)
            {
                if(pawn.def.HasModExtension<DefModExt_Fireproof>() && dinfo.Def == DamageDefOf.Flame)
                {
                    __result = new DamageWorker.DamageResult();
                    return false;
                }
            }

            return true;
        }
    }
}
