using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
	[HarmonyPatch(typeof(Building), "SpawnSetup")]
	internal static class SpawnSetup_Patch
	{
		private static void Postfix(Building __instance)
		{
			var options = __instance.def.GetModExtension<BuildingSpawnOptions>();
			if (options != null)
            {
				if (options.materialReplaces != null)
                {
					var terrainDef = __instance.Position.GetTerrain(__instance.Map);
					foreach (var materialReplace in options.materialReplaces)
                    {
						if (materialReplace.floorDef == terrainDef)
                        {
							if (__instance.def != materialReplace.replaceWithThingDef)
                            {
								var stuff = materialReplace.replaceWithStuffDef != null ? materialReplace.replaceWithStuffDef : GenStuff.DefaultStuffFor(materialReplace.replaceWithThingDef);
								SpawnInsteadOf(__instance, materialReplace.replaceWithThingDef, stuff);
							}
							else if (__instance.Stuff != null && materialReplace.replaceWithStuffDef != null && __instance.Stuff != materialReplace.replaceWithStuffDef)
                            {
								SpawnInsteadOf(__instance, __instance.def, materialReplace.replaceWithStuffDef);
							}
                        }
                    }
                }
			}
		}

		private static void SpawnInsteadOf(Building oldBuilding, ThingDef newBuildingDef, ThingDef stuff)
        {
			var map = oldBuilding.Map;
			var position = oldBuilding.Position;
			var rotation = oldBuilding.Rotation;
			var newBuilding = ThingMaker.MakeThing(newBuildingDef, stuff);
			newBuilding.SetFaction(oldBuilding.Faction);
			oldBuilding.Destroy();
			GenPlace.TryPlaceThing(newBuilding, position, map, ThingPlaceMode.Direct, null, null, rotation);
		}
	}
}
