using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class TerrainThresholdWEO
	{
		public TerrainDef terrain;

		public float min = -1000f;

		public float max = 1000f;

		public List<TerrainDef> excludeOverwrite;

		public static TerrainDef TerrainAtValue(List<TerrainThresholdWEO> threshes, float val, TerrainDef current)
		{
			TerrainDef result;
			if (threshes == null)
			{
				result = null;
			}
			else
			{
				int i = 0;
				while (i < threshes.Count)
				{
					if (threshes[i].min <= val && threshes[i].max >= val)
					{
						if (threshes[i].excludeOverwrite != null && threshes[i].excludeOverwrite.Contains(current))
						{
							return null;
						}
						return threshes[i].terrain;
					}
					else
					{
						i++;
					}
				}
				result = null;
			}
			return result;
		}
	}
}
