﻿using RimWorld;
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
    [HarmonyPatch(typeof(MainTabWindow_Research), "VisibleResearchProjects", MethodType.Getter)]
    public static class Patch_MainTabWindow_Research_VisibleResearchProjects
    {
        public static bool resolved;

        [HarmonyPrefix]
        public static void Prefix(MainTabWindow_Research __instance)
        {
            if(__instance.cachedVisibleResearchProjects == null)
            {
                resolved = false;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(MainTabWindow_Research __instance, ref List<ResearchProjectDef> __result)
        {
            if (!resolved)
            {
                foreach(ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs.Where(rp => rp.HasModExtension<DefModExt_HiddenResearch>()))
                {
                    if (__instance.cachedVisibleResearchProjects.Contains(def))
                    {
                        __instance.cachedVisibleResearchProjects.Remove(def);
                    }
                }
                resolved = true;
            }
        }
    }
}
