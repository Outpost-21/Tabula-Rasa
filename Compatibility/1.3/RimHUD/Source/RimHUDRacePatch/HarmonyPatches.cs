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

    [HarmonyPatch(typeof(PawnModel), "GetRaceIfNotHuman")]
    public static class PawnModel_GetRaceIfNotHuman
    {
        [HarmonyPrefix]
        public static bool Prefix(ref string __result, PawnModel __instance)
        {
            __result = __instance.Base.RaceProps?.Humanlike ?? true ? null : __instance.Base.def?.label;
            return false;
        }
    }
}
