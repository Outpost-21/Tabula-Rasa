using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Culture;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(Ideo), "SetIcon")]
    public static class Patch_IdeoFoundation_GetRandomIconDef
    {
        [HarmonyPostfix]
        public static void PostFix(Ideo __instance, IdeoIconDef iconDef, ColorDef colorDef, bool clearPrimaryFactionColor)
        {
            if (__instance.culture != null && __instance.culture.HasModExtension<DefModExt_CultureExtended>())
            {
                DefModExt_CultureExtended ext = __instance.culture.GetModExtension<DefModExt_CultureExtended>();

                if(ext.ideoIconDef != null)
                {
                    __instance.iconDef = ext.ideoIconDef;
                }
                if (ext.ideoIconColor != null)
                {
                    __instance.colorDef = ext.ideoIconColor;
                }
            }
        }
    }
}
