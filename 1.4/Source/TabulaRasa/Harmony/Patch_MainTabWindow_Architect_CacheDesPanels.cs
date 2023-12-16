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
    [HarmonyPatch(typeof(MainTabWindow_Architect), "CacheDesPanels")]
    public static class Patch_MainTabWindow_Architect_CacheDesPanels
    {
        [HarmonyPostfix]
        public static void Postfix(MainTabWindow_Architect __instance)
        {
            List<ArchitectCategoryTab> removals = new List<ArchitectCategoryTab>();
            foreach(ArchitectCategoryTab tab in __instance.desPanelsCached)
            {
                if (tab.def.HasModExtension<DefModExt_HideArchitectTab>())
                {
                    removals.Add(tab);
                }
            }
            foreach(ArchitectCategoryTab tab in removals)
            {
                __instance.desPanelsCached.Remove(tab);
            }
        }
    }
}
