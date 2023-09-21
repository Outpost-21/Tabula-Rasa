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
    [HarmonyPatch(typeof(WildPlantSpawner), "CheckSpawnWildPlantAt")]
    public static class Patch_WildPlantSpawner_CheckSpawnWildPlantAt
    {
        [HarmonyPrefix]
        public static bool Prefix(WildPlantSpawner __instance, IntVec3 c, float plantDensity, float wholeMapNumDesiredPlants, bool setRandomGrowth = false)
        {
            if (c.GetThingList(__instance.map).Any(t => t.def.HasModExtension<DefModExt_PreventPlantSpawns>()))
            {
                return false;
            }
            return true;
        }
    }
}
