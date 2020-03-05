using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class DefModExt_Biome_Islands : DefModExt
	{
		public float baseFrequency = 0.02f;

		public int islandCountMin = 0;

		public int islandCountMax = 0;

		public float minSizeX = 0.2f;

		public float maxSizeX = 0.3f;

		public float minSizeZ = 0.2f;

		public float maxSizeZ = 0.3f;

		public List<SimpleCurve> coneValueCurves = null;

		public GenStepCalculationType calcElevationType = GenStepCalculationType.None;

		public float noiseElevationPreScale = 1f;

		public float noiseElevationPreOffset = 0f;

		public List<SimpleCurve> elevationPostCurves = null;

		public GenStepCalculationType calcFertilityType = GenStepCalculationType.None;

		public float noiseFertilityPreScale = 1f;

		public float noiseFertilityPreOffset = 0f;

		public List<SimpleCurve> fertilityPostCurves = null;
	}
}
}
