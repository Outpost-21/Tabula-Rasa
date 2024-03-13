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
    [HarmonyPatch(typeof(CompSchedule), "RecalculateAllowed")]
    public static class Patch_CompSchedule_RecalculateAllowed
    {
        [HarmonyPrefix]
        public static bool Prefix(CompSchedule __instance)
        {
            float num = GenLocalDate.DayPercent(__instance.parent);
            if (__instance.parent.def.HasModExtension<DefModExt_Nightlight>())
            {
                __instance.Allowed = !(num > __instance.Props.startTime && num < __instance.Props.endTime);
                return false;
            }
            return true;
        }
    }
}
