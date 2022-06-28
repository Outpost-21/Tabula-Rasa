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
    [HarmonyPatch(typeof(CompAffectedByFacilities), "Notify_NewLink")]
    public static class Patch_CompAffectedByFacilities_Notify_NewLink
    {
        [HarmonyPostfix]
        public static void Postfix(Thing facility, CompAffectedByFacilities __instance)
        {
            Comp_RecipesFromFacilities.AddRecipe(facility, __instance);
        }
    }
}
