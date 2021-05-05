using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Currency;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(Thing), "MarketValue", MethodType.Getter)]
    public class Patch_Thing_MarketValue
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, ref Thing __instance)
        {
            if (__instance.def.HasModExtension<DefModExt_Currency>())
            {
                DefModExt_Currency modExt = __instance.def.GetModExtension<DefModExt_Currency>();
                __result = __instance.def.BaseMarketValue;
            }
        }
    }

    [HarmonyPatch(typeof(Tradeable), "GetPriceFor")]
    public class Patch_Tradeable_GetPriceFor
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, ref Tradeable __instance, TradeAction action)
        {
            if (__instance.ThingDef.HasModExtension<DefModExt_Currency>())
            {
                //DefModExt_Currency modExt = __instance.ThingDef.GetModExtension<DefModExt_Currency>();
                __result = __instance.ThingDef.BaseMarketValue;

            }
        }
    }
}
