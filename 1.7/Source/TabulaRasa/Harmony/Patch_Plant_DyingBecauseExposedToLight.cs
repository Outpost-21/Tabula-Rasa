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
    [HarmonyPatch(typeof(Plant), "DyingBecauseExposedToLight", MethodType.Getter)]
    public static class Patch_Plant_DyingBecauseExposedToLight
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref bool __result)
        {
            DefModExt_PlantStuff modExt = __instance.def.GetModExtension<DefModExt_PlantStuff>();
            if (modExt == null) { return; }
            if (__result)
            {
                bool isInSunlight = __instance.Map.glowGrid.GroundGlowAt(__instance.Position, true) > 0f;
                if (modExt.diesInSunlight && isInSunlight)
                {
                    __result = true;
                    return;
                }
                if (modExt.diesInDarklight != null)
                {
                    bool isInDarklight = DarklightUtility.IsDarklightAt(__instance.Position, __instance.Map);
                    if ((bool)modExt.diesInDarklight && isInDarklight)
                    {
                        __result = true;
                        return;
                    }
                    else if (!(bool)modExt.diesInDarklight && isInDarklight)
                    {
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
}
