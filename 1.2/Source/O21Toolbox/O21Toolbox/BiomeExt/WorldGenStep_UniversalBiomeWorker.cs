using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Noise;

namespace O21Toolbox.BiomeExt
{
    public class WorldGenStep_UniversalBiomeWorker : WorldGenStep
    {
		public static ModuleBase PerlinNoise = null;

		public bool validForPrinting = true;

        public override int SeedPart => 123456789;

        public override void GenerateFresh(string seed)
        {
            UniversalBiomeWorker();
        }

        private void UniversalBiomeWorker()
        {
            IEnumerable tweakedBiomes = from x in DefDatabase<BiomeDef>.AllDefsListForReading
                                        where x.HasModExtension<DefModExt_BiomeWorker>()
                                        select x;

            foreach (BiomeDef biomeDef in tweakedBiomes)
            {
                DefModExt_BiomeWorker modExt = biomeDef.GetModExtension<DefModExt_BiomeWorker>();
				float minSouthLatitude = modExt.minSouthLatitude * -1f;
				float maxSouthLatitude = modExt.maxSouthLatitude * -1f;
				for (int tileID = 0; tileID < Find.WorldGrid.TilesCount; tileID++)
				{
					float latitude = Find.WorldGrid.LongLatOf(tileID).y;
					int perlinSeed = Find.World.info.Seed;
					PerlinNoise = new Perlin(0.1, 10.0, 0.6, 12, perlinSeed, QualityMode.Low);
					Vector3 coords = Find.WorldGrid.GetTileCenter(tileID);
					float perlinNoiseValue = PerlinNoise.GetValue(coords);
					Tile tile = Find.WorldGrid[tileID];
					bool validTarget = true;
					foreach (BiomeDef targetBiome in modExt.placeInBiomes)
					{
						if (tile.biome == targetBiome)
						{
							validTarget = true;
							break;
						}
						validTarget = false;
					}
					if (validTarget)
					{
						if (!(latitude < minSouthLatitude && latitude > maxSouthLatitude) && !(latitude > modExt.minNorthLatitude && latitude < modExt.maxNorthLatitude))
						{
							if (modExt.minSouthLatitude != -9999f && modExt.minNorthLatitude != -9999f && modExt.maxSouthLatitude != -9999f && modExt.maxNorthLatitude != 9999f)
							{
								goto IL_EndPoint;
							}
						}
						if (modExt.perlinCustomSeed != null)
						{
							perlinSeed = modExt.perlinCustomSeed.Value;
						}
						else
						{
							if (modExt.useAlternativePerlinSeedPreset)
							{
							}
						}
						if (!(tile.WaterCovered && !modExt.allowOnWater))
						{
							if (!(!tile.WaterCovered && !modExt.allowOnLand))
							{
								if (modExt.needRiver)
								{
									if (tile.Rivers == null || tile.Rivers.Count == 0)
									{
										goto IL_EndPoint;
									}
								}
								if (modExt.usePerlin)
								{
									if (perlinNoiseValue > modExt.perlinCulling / 100f)
									{
										goto IL_EndPoint;
									}
								}
								if (!((double)Rand.Value > Math.Pow((double)modExt.frequency, 2.0) / 10000.0))
								{
									if (!(tile.elevation < modExt.minElevation || tile.elevation > modExt.maxElevation))
									{
										if (!(tile.temperature < modExt.minTemperature || tile.temperature > modExt.maxTemperature))
										{
											if (!(tile.rainfall < modExt.minRainfall || tile.rainfall > modExt.maxRainfall))
											{
												if (!(tile.hilliness < modExt.minHilliness || tile.hilliness > modExt.maxHilliness))
												{
													if (modExt.randomizeHilliness)
													{
														switch (Rand.Range(0, 4))
														{
															case 0:
																tile.hilliness = Hilliness.Flat;
																break;
															case 1:
																tile.hilliness = Hilliness.SmallHills;
																break;
															case 2:
																tile.hilliness = Hilliness.LargeHills;
																break;
															case 3:
																tile.hilliness = Hilliness.Mountainous;
																break;
														}
													}
													tile.biome = biomeDef;
													if (modExt.spawnHills != null)
													{
														tile.hilliness = modExt.spawnHills.Value;
													}
												}
											}
										}
									}
								}
							}
						}
					}
					IL_EndPoint:;
				}
			}
        }
    }
}
