using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

using RimHUD.Data.Models;

namespace RimHUDRacePatch
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static Harmony harmony;

        static HarmonyPatches()
        {
            harmony = new Harmony("neronix17.rimhudracepatch.rimworld");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(PawnModel), "GetRace", MethodType.Normal)]
    public static class PawnModel_GetRaceIfNotHuman
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, PawnModel __instance)
        {
            string result = null;
            if ((bool)__instance.Base.RaceProps?.Humanlike)
            {
                result = __instance.Base.def?.label ?? null;
            }
            if(result != null)
            {
                __result = result;
            }
        }
    }
}
