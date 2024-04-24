using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public static class WaterPlantsUtil
    {
		public static Map map;

		public static int cycle;

        public static void TryGenerateWaterPlants(Map map)
		{
			foreach (IntVec3 cell in map.AllCells)
			{
				if (Rand.Chance(map.wildPlantSpawner.CachedChanceFromDensity))
				{
					TrySpawnWaterPlant(map, cell, true);
				}
			}
		}

        public static void TrySpawnWaterPlants(Map map)
        {
			if (map?.AllCells != null || !map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow) { return; }
            if(map.wildPlantSpawner.CurrentWholeMapNumDesiredPlants <= 0.0f) { return; }
			int area = map.Area;
			if (map != WaterPlantsUtil.map)
			{
                WaterPlantsUtil.map = map;
                cycle = Rand.Range(0, area);
			}
			int num = Mathf.CeilToInt(area * 0.0001f);
            for (int i = 0; i < num; i++)
			{
				if (cycle >= area)
				{
					cycle = 0;
				}
				IntVec3 c = map.cellsInRandomOrder.Get(cycle);
				if (Rand.Chance(map.wildPlantSpawner.CachedChanceFromDensity) && Rand.MTBEventOccurs(map.Biome.wildPlantRegrowDays, 60000f, 10000f))
                {
                    TrySpawnWaterPlant(map, c, true);
                }
				cycle++;
            }
        }

        public static void TrySpawnWaterPlant(Map map, IntVec3 cell, bool randomGrowth = false)
		{
			TerrainDef terrain = cell.GetTerrain(map);
			if (!terrain.IsWater || terrain.passability != Traversability.Standable) { return; }
			List<ThingDef> growablePlants = map.Biome.AllWildPlants.Where((ThingDef x) => x.IsWaterPlant()).ToList();
			if (growablePlants.NullOrEmpty()) { return; }
			growablePlants.TryRandomElementByWeight((ThingDef x) => map.Biome.CommonalityOfPlant(x), out var randPlant);
			if (randPlant == null) { return; }

			DefModExt_PlantStuff modExt = randPlant.GetModExtension<DefModExt_PlantStuff>();
			if (modExt.freshWaterPlant && !CanGrowFreshWaterPlants(cell, map)) { return; }
			else if (modExt.oceanWaterPlant && !CanGrowOceanWaterPlants(cell, map)) { return; }
			Plant closest = (Plant)GenClosest.ClosestThing_Global(cell, map.listerThings.ThingsMatching(new ThingRequest() { singleDef = randPlant }), modExt.distToNearestOther);
			if(closest != null) { return; }
			Plant plant = (Plant)ThingMaker.MakeThing(randPlant);
			if (randomGrowth)
			{
				plant.Growth = Rand.Range(0.2f, 1f);
			}
			GenSpawn.Spawn(plant, cell, map);
		}

		public static bool CanGrowFreshWaterPlants(IntVec3 cell, Map map)
		{
			if (map == null || cell.CloseToEdge(map, 3) || cell.Roofed(map) || map.thingGrid.ThingsListAt(cell).Any() || !PlantUtility.SnowAllowsPlanting(cell, map))
			{
				return false;
			}
			TerrainDef terrain = cell.GetTerrain(map);
			return !terrain.bridge && !terrain.defName.Contains("Deep");
		}

		public static bool CanGrowOceanWaterPlants(IntVec3 cell, Map map)
		{
			if (map == null || cell.CloseToEdge(map, 3) || cell.Roofed(map) || map.thingGrid.ThingsListAt(cell).Any() || !PlantUtility.SnowAllowsPlanting(cell, map))
			{
				return false;
			}
			TerrainDef terrain = cell.GetTerrain(map);
			return !terrain.bridge && (terrain.defName.Contains("Ocean") || terrain.defName.Contains("Deep"));
		}

		public static bool AnyWaterPlants()
		{
			return DefDatabase<ThingDef>.AllDefsListForReading.Any((ThingDef x) => (x.IsWaterPlant())); 
		}

		public static bool IsWaterPlant(this ThingDef thing)
        {
			DefModExt_PlantStuff modExt = thing.GetModExtension<DefModExt_PlantStuff>();
			return modExt != null && (modExt.freshWaterPlant || modExt.oceanWaterPlant);
        }
	}
}
