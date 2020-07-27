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
    public class DefModExt_BiomeWorker : DefModExtension
    {
        /// <summary>
        /// Placement Settings
        /// </summary>

        public bool allowOnLand = false;

        public bool allowOnWater = false;

        public bool needRiver = false;

        public bool randomizeHilliness = false;

        public float minTemperature = -999f;

        public float maxTemperature = 999f;

        public float minElevation = -9999f;

        public float maxElevation = 9999f;

        public float minNorthLatitude = -9999f;

        public float maxNorthLatitude = -9999f;

        public float minSouthLatitude = -9999f;

        public float maxSouthLatitude = -9999f;

        public Hilliness minHilliness = Hilliness.Flat;

        public Hilliness maxHilliness = Hilliness.Impassable;

        public Hilliness? spawnHills = null;

        public float minRainfall = -9999f;

        public float maxRainfall = 9999f;

        public int frequency = 100;

        public bool useAlternativePerlinSeedPreset = false;

        public List<BiomeDef> placeInBiomes = new List<BiomeDef>();

        /// <summary>
        /// Noise map Settings
        /// </summary>

        public bool usePerlin = false;

        public int? perlinCustomSeed = null;

        public float perlinCulling = 0.5f;

        public double perlinFrequency;

        public double perlinLacunarity;

        public double perlinPersistence;

        public int perlinOctaves;

        /// <summary>
        /// Visual Settings
        /// </summary>
        
        public string materialPath = "World/MapGraphics/Default";

        public int materialLayer = 3515;
    }
}
