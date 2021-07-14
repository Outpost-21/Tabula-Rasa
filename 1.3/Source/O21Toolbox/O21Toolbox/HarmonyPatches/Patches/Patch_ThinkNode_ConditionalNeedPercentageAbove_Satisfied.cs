using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(ThinkNode_ConditionalNeedPercentageAbove), "Satisfied")]
    public class Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied
    {

        [HarmonyPrefix]
        public static bool Prefix(ref ThinkNode_ConditionalNeedPercentageAbove __instance, ref bool __result, ref Pawn pawn)
        {
            NeedDef need = __instance.need;
            bool haveNeed = pawn.needs.TryGetNeed(need) != null;

            if (!haveNeed)
            {
                __result = false;
                return false;
            }

            return true;
        }
        public static NeedDef ThinkNode_ConditionalNeedPercentageAbove_GetNeed(ThinkNode_ConditionalNeedPercentageAbove instance)
        {
            return (NeedDef)instance.need;
        }
    }
}
