using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.AutoHeal;
using O21Toolbox.Health;

namespace O21Toolbox.HarmonyPatches
{

    [HarmonyPatch(typeof(Pawn_IdeoTracker), "CertaintyChangePerDay", MethodType.Getter)]
    static class Patch_Pawn_IdeoTracker_CertaintyChangePerDay
    {
        [HarmonyPrefix]
        static bool Prefix(Pawn_IdeoTracker __instance, float __result)
        {
            if (__instance.pawn.needs.mood == null)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
