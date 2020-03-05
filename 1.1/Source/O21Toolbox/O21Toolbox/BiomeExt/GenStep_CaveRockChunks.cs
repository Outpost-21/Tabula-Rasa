using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Noise;

namespace O21Toolbox.BiomeExt
{
    public class GenStep_CaveRockChunks : GenStep
	{
		private ModuleBase freqFactorNoise;

		private const float ThreshLooseRock = 0.55f;

		private const float PlaceProbabilityPerCell = 0.006f;

		private const float RubbleProbability = 0.2f;

		private static readonly IntRange MaxRockChunksPerGroup = new IntRange(1, 6);

		public override int SeedPart
		{
			get
			{
				return 488758298;
			}
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00024D20 File Offset: 0x00022F20
		public override void Generate(Map map, GenStepParams parms)
		{
			bool waterCovered = map.TileInfo.WaterCovered;
			if (!waterCovered)
			{
				DefModExt_Biome_FeatureControl modExtension = map.Biome.GetModExtension<DefModExt_Biome_FeatureControl>();
				bool flag = modExtension == null || modExtension.overwriteRockChunks != RockChunksOverwriteType.AddToCaves;
				if (!flag)
				{
					this.freqFactorNoise = new Perlin(0.014999999664723873, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
					this.freqFactorNoise = new ScaleBias(1.0, 1.0, this.freqFactorNoise);
					NoiseDebugUI.StoreNoiseRender(this.freqFactorNoise, "cave_rock_chunks_freq_factor");
					MapGenFloatGrid elevation = MapGenerator.Elevation;
					foreach (IntVec3 intVec in map.AllCells)
					{
						float num = 0.006f * this.freqFactorNoise.GetValue(intVec);
						bool flag2 = elevation[intVec] >= 0.55f && Rand.Value < num;
						if (flag2)
						{
							this.GrowLowRockFormationFrom(intVec, map);
						}
					}
					this.freqFactorNoise = null;
				}
			}
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00024E60 File Offset: 0x00023060
		private void GrowLowRockFormationFrom(IntVec3 root, Map map)
		{
			ThingDef filth_RubbleRock = ThingDefOf.Filth_RubbleRock;
			ThingDef mineableThing = Find.World.NaturalRockTypesIn(map.Tile).RandomElement<ThingDef>().building.mineableThing;
			Rot4 random = Rot4.Random;
			MapGenFloatGrid elevation = MapGenerator.Elevation;
			IntVec3 intVec = root;
			int randomInRange = GenStep_CaveRockChunks.MaxRockChunksPerGroup.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				intVec += Rot4Utility.RandomButExclude(random).FacingCell;
				bool flag = !intVec.InBounds(map) || intVec.GetEdifice(map) != null || intVec.GetFirstItem(map) != null;
				if (flag)
				{
					break;
				}
				bool flag2 = elevation[intVec] < 0.55f;
				if (flag2)
				{
					break;
				}
				List<TerrainAffordanceDef> affordances = map.terrainGrid.TerrainAt(intVec).affordances;
				bool flag3 = !affordances.Contains(TerrainAffordanceDefOf.Medium) && !affordances.Contains(TerrainAffordanceDefOf.Heavy);
				if (flag3)
				{
					break;
				}
				GenSpawn.Spawn(mineableThing, intVec, map, WipeMode.Vanish);
				foreach (IntVec3 b in GenAdj.AdjacentCellsAndInside)
				{
					bool flag4 = Rand.Value < 0.2f;
					if (!flag4)
					{
						IntVec3 intVec2 = intVec + b;
						bool flag5 = intVec2.InBounds(map);
						if (flag5)
						{
							bool flag6 = false;
							List<Thing> thingList = intVec2.GetThingList(map);
							for (int k = 0; k < thingList.Count; k++)
							{
								Thing thing = thingList[k];
								bool flag7 = thing.def.category != ThingCategory.Plant && thing.def.category != ThingCategory.Item && thing.def.category != ThingCategory.Pawn;
								if (flag7)
								{
									flag6 = true;
									break;
								}
							}
							bool flag8 = !flag6;
							if (flag8)
							{
								FilthMaker.MakeFilth(intVec2, map, filth_RubbleRock, 1);
							}
						}
					}
				}
			}
		}
	}
}
