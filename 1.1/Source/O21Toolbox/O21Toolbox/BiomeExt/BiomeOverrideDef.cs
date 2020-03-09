using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class BiomeOverrideDef : Def
    {
        /// <summary>
        /// List of Biomes the override(s) will apply to.
        /// </summary>
        public List<BiomeDef> biomeDefs = new List<BiomeDef>();

        /// <summary>
        /// Used to override items generated on the map. Like Rock Chunks
        /// </summary>
        public List<BiomeThingOverrides> thingOverrides = new List<BiomeThingOverrides>();

        /// <summary>
        /// Used to override terrain tiles, like soil, sand or water.
        /// </summary>
        public List<BiomeTerrainOverrides> terrainOverrides = new List<BiomeTerrainOverrides>();

        /// <summary>
        /// Used to override an edifice, such as rock walls.
        /// </summary>
        public List<BiomeEdificeOverrides> edificeOverrides = new List<BiomeEdificeOverrides>();
        
        public class BiomeThingOverrides
        {
            /// <summary>
            /// Original Thing to replace.
            /// </summary>
            public ThingDef oldThing = null;

            /// <summary>
            /// New thing to replace old thing with.
            /// </summary>
            public ThingDef newThing = null;
        }

        public class BiomeTerrainOverrides
        {
            /// <summary>
            /// Original tile to replace.
            /// </summary>
            public TerrainDef oldTerrain = null;

            /// <summary>
            /// New tile to replace old tile with.
            /// </summary>
            public TerrainDef newTerrain = null;
        }

        public class BiomeEdificeOverrides
        {
            /// <summary>
            /// Original Edifice to replace.
            /// </summary>
            public ThingDef oldEdifice = null;

            /// <summary>
            /// New Edifice to replace old Edifice with.
            /// </summary>
            public ThingDef newEdifice = null;
        }
    }
}
