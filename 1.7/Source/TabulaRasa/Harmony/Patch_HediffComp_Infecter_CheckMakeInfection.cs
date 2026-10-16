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
    [HarmonyPatch(typeof(HediffComp_Infecter), "CheckMakeInfection")]
    public static class Patch_HediffComp_Infecter_CheckMakeInfection
    {
        [HarmonyPrefix]
        public static bool PreFix(ref HediffComp_Infecter __instance)
        {
            DefModExt_RaceProperties raceProps = __instance.Pawn.def.GetModExtension<DefModExt_RaceProperties>();
            if(raceProps != null && !raceProps.infectionsEnabled)
            {
                return false;
            }
            return true;
        }
    }
}
