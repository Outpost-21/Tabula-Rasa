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
    [HarmonyPatch(typeof(ArchitectCategoryTab), nameof(ArchitectCategoryTab.DesignationTabOnGUI))]
    public static class Patch_ArchitectCategoryTab_DesignationTabOnGUI
    {
        [HarmonyPrefix]
        public static bool Prefix(ArchitectCategoryTab __instance, Designator forceActivatedCommand)
        {
            if (__instance.def.HasModExtension<DefModExt_SubcategoryDisplay>())
            {
                SubcategoryUtil.PopulateArchitectCategoryTab(__instance);
                SubcategoryUtil.DrawSubcategoryWindow(__instance, forceActivatedCommand);
                return false;
            }
            return true;
        }
    }
}
