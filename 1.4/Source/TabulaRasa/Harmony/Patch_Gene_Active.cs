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
    [HarmonyPatch(typeof(Gene), "Active", MethodType.Getter)]
    public static class Patch_Gene_Active
    {
        [HarmonyPrefix]
        public static bool PreFix(Gene __instance, Pawn ___pawn, ref bool __result)
        {
            if((__instance as Gene_MaleOnly != null) && ___pawn.gender != Gender.Male)
            {
                __result = false;
                return false;
            }
            if((__instance as Gene_FemaleOnly != null) && ___pawn.gender != Gender.Female)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
