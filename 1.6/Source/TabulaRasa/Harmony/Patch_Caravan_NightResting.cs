using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.Planet;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Caravan), "NightResting", MethodType.Getter)]
    public static class Patch_Caravan_NightResting
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref Caravan __instance)
        {
            if (__instance.pawns.InnerListForReading.Any((Pawn pawn) => pawn.def.race.needsRest))
            {
                return true;
            }
            __result = false;
            return false;
        }
    }
}
