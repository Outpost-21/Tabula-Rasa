using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.BiomeExt.BiomeWorkerSpecial
{
    public class CaveEntrance : BiomeWorkers.Special
	{
		private const int BiomeChangeDigLengthMin = 3;

		private const float TunnelBiomeChance = 0.85f;

		private const float OasisBiomeChance = 0.15f;

		private static readonly IntRange BiomeChangeDigLengthMax = new IntRange(8, 30);

		protected override float InitialGenChance
		{
			get
			{
				return 0.07f;
			}
		}

		protected override float GenChanceOffsetAfterFirstHit
		{
			get
			{
				return 0.03f;
			}
		}

		protected override float GenChancePerHitFactor
		{
			get
			{
				return 0.9f;
			}
		}

		public override bool PreRequirements(Tile tile)
		{
			return !tile.WaterCovered && tile.hilliness != Hilliness.Flat;
		}

		public override void PostGeneration(int tileID)
		{
			int randomInRange = CaveEntrance.BiomeChangeDigLengthMax.RandomInRange;
			base.DigTilesForBiomeChange(tileID, 3, randomInRange, 2, true);
		}

		protected override void ChangeTileAfterSuccessfulDig(Tile tile, bool end)
		{
			float value = Rand.Value;
			if (value < 0.15f || end)
			{
				tile.biome = BiomeDefOf.CaveOasis;
			}
			else if (value < 0.85f)
			{
				tile.biome = BiomeDefOf.TunnelworldCave;
			}
			else
			{
				tile.biome = BiomeDefOf.CaveEntrance;
			}
			GenWorldGen.UpdateTileByBiomeModExts(tile);
		}
	}
}
