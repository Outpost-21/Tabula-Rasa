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
    [HarmonyPatch(typeof(Map), "MapPostTick")]
    public static class Patch_Map_MapPostTick
	{
        [HarmonyPostfix]
        public static void Postfix(Map __instance)
		{
            if (__instance.IsPlayerHome)
            {
                WaterPlantsUtil.TrySpawnWaterPlants(__instance);
            }
		}
    }
}
