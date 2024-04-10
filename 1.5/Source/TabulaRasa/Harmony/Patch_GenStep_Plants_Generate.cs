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
    [HarmonyPatch(typeof(GenStep_Plants), "Generate")]
    public static class Patch_GenStep_Plants_Generate
    {
        [HarmonyPostfix]
        public static void Postfix(Map map)
		{
            if (map != null)
            {
                WaterPlantsUtil.TryGenerateWaterPlants(map);
            }
		}
    }
}
