using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class GenWorldGen
	{
		public static void UpdateTileByBiomeModExts(Tile tile)
		{
			Biome_Replacement modExtension = tile.biome.GetModExtension<Biome_Replacement>();
			if (modExtension != null)
			{
				if (modExtension.replaceElevation)
				{
					tile.elevation = (float)Rand.RangeInclusive(modExtension.elevationMin, modExtension.elevationMax);
				}
				if (modExtension.replaceHilliness != null)
				{
					tile.hilliness = modExtension.replaceHilliness.Value;
				}
			}
			Biome_Temperature modExtension2 = tile.biome.GetModExtension<Biome_Temperature>();
			if (modExtension2 != null)
			{
				tile.temperature = tile.temperature * (1f - modExtension2.tempStableWeight) + modExtension2.tempStableValue * modExtension2.tempStableWeight + modExtension2.tempOffset;
			}
		}

		public static int NextRandomDigDir(int dir, int step)
		{
			step = Mathf.Clamp(step, 1, 3);
			dir += Rand.RangeInclusive(-step, step);
			if (dir < 0)
			{
				dir += 6;
			}
			if (dir > 5)
			{
				dir -= 6;
			}
			return dir;
		}

		public static int InvertDigDir(int dir)
		{
			dir += 3;
			if (dir > 5)
			{
				dir -= 6;
			}
			return dir;
		}
	}
}
