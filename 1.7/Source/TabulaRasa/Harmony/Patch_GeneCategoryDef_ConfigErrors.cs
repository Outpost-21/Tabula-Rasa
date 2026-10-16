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
    /// <summary>
    /// Fixes overlapping display priorities where two mods share the same priority before it can error.
    /// This is something that definitely shouldn't need to be done manually, Ludeon.
    /// </summary>
    [HarmonyPatch(typeof(GeneCategoryDef), "ConfigErrors")]
    public static class Patch_GeneCategoryDef_ConfigErrors
    {
        [HarmonyPrefix]
        public static void Prefix(GeneCategoryDef __instance)
        {
            while (DefDatabase<GeneCategoryDef>.AllDefs.Any((GeneCategoryDef x) => x != __instance && x.displayPriorityInXenotype == __instance.displayPriorityInXenotype))
            {
                __instance.displayPriorityInXenotype++;
            }
        }
    }
}
