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
    [HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
    public static class Patch_WildPlantSpawner_CalculatePlantsWhichCanGrowAt
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<ThingDef> outPlants)
		{
			if (outPlants.NullOrEmpty()) { return; }
			List<ThingDef> removals = new List<ThingDef>();
			for (int i = 0; i < outPlants.Count; i++)
			{
				if (WaterPlantsUtil.IsWaterPlant(outPlants[i]))
				{
					removals.Add(outPlants[i]);
				}
			}
			outPlants.RemoveAll(td => removals.Contains(td));
		}
    }
}
