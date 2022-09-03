using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.Planet;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Designator_Build), "Visible", MethodType.Getter)]
    public static class Patch_Designator_Build_Visible
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator_Build __instance, ref bool __result)
		{
			if (__instance.entDef.PlaceWorkers != null)
			{
				foreach (PlaceWorker placeWorker in __instance.entDef.PlaceWorkers)
				{
					PlaceWorker_SubCategoryBuilding subCat = placeWorker as PlaceWorker_SubCategoryBuilding;
					if(subCat != null && !subCat.IsBuildDesignatorVisible(__instance.entDef))
                    {
						__result = false;
						return false;
                    }

					PlaceWorker_SubCategoryBuildingOnly subCatOnly = placeWorker as PlaceWorker_SubCategoryBuildingOnly;
					if (subCatOnly != null && !subCatOnly.IsBuildDesignatorVisible(__instance.entDef))
					{
						__result = false;
						return false;
					}
				}
			}
			return true;
        }
    }
}
