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
    public class Biome_FeatureControl
    {
        public GenFeatureTristate overwriteCaves = GenFeatureTristate.Default;

		public HiveOverwriteType overwriteHives = HiveOverwriteType.Default;

		public int additionalHivesScaling = 0;

		public RoofOverwriteType overwriteRoof = RoofOverwriteType.None;

		public RoomCalculationType roomCalculationType = RoomCalculationType.Default;

		public bool removeBeach = false;

		public RockChunksOverwriteType overwriteRockChunks = RockChunksOverwriteType.Default;

		public bool removeRuinsSimple = false;

		public bool removeShrines = false;
	}
}
