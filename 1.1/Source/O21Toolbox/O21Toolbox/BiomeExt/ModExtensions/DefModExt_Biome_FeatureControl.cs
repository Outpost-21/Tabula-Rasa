using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class DefModExt_Biome_FeatureControl : DefModExt
    {
		public static DefModExt_Biome_FeatureControl Default
		{
			get
			{
				bool flag = DefModExt_Biome_FeatureControl.defaultInt == null;
				if (flag)
				{
					DefModExt_Biome_FeatureControl.defaultInt = new DefModExt_Biome_FeatureControl();
				}
				return DefModExt_Biome_FeatureControl.defaultInt;
			}
		}
		public List<WildPlantWorker> WildPlantWorkers
		{
			get
			{
				bool flag = this.wildPlantWorkersInstantiatedInt == null;
				if (flag)
				{
					this.wildPlantWorkersInstantiatedInt = new List<WildPlantWorker>();
					bool flag2 = this.wildPlantWorkers != null;
					if (flag2)
					{
						foreach (Type type in this.wildPlantWorkers)
						{
							this.wildPlantWorkersInstantiatedInt.Add((WildPlantWorker)Activator.CreateInstance(type));
						}
					}
				}
				return this.wildPlantWorkersInstantiatedInt;
			}
		}

		private static DefModExt_Biome_FeatureControl defaultInt;

		public BiomeFeatureType biomeType = BiomeFeatureType.Normal;

		public GenFeatureTristate overwriteCaves = GenFeatureTristate.Default;

		public HiveOverwriteType overwriteHives = HiveOverwriteType.Default;

		public int additionalHivesScaling = 0;

		public RoofOverwriteType overwriteRoof = RoofOverwriteType.None;

		public RoomCalculationType roomCalculationType = RoomCalculationType.Default;

		public BeachOverwriteType overwriteBeach = BeachOverwriteType.None;

		public float beachHeight = 1f;

		public bool edgeWaterToOcean = false;

		public bool smoothCoastTerrain = false;

		public RockChunksOverwriteType overwriteRockChunks = RockChunksOverwriteType.Default;

		public bool removeRuinsSimple = false;

		public bool removeShrines = false;

		public bool addExtraCoal = false;

		public List<Type> wildPlantWorkers;

		private List<WildPlantWorker> wildPlantWorkersInstantiatedInt;
	}
}
