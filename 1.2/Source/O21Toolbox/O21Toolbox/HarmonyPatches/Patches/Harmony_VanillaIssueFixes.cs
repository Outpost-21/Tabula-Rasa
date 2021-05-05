using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(CompSchedule), "RecalculateAllowed")]
    public class Patch_CompSchedule_RecalculateAllowed
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
