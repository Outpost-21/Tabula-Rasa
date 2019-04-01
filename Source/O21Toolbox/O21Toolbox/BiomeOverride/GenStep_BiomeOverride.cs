using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeOverride
{
    public class GenStep_BiomeOverride : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1370184742;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            bool overrideLogging = true;
            if (overrideLogging) { Log.Message("Fuck Tynan for making custom biomes so nasty to create. Procedural Generation isn't the be all and end all of everything you Canuck fuck."); }
            IEnumerable<BiomeOverrideDef> enumerable = from def in DefDatabase<BiomeOverrideDef>.AllDefs
                                                       where def.biomeDefs != null
                                                       select def;
            foreach (BiomeOverrideDef current in enumerable)
            {
                if (current.biomeDefs.Contains(map.Biome))
                {
                    if(current.edificeOverrides != null)
                    {
                        ReplaceEdifice(map, current);
                    }
                    if(current.edificeOverrides == null && overrideLogging) { Log.Message("Skipping Edifice Overrides for: " + current.defName); }
                    if(current.terrainOverrides != null)
                    {
                        ReplaceTerrain(map, current);
                    }
                    if (current.terrainOverrides == null && overrideLogging) { Log.Message("Skipping Terrain Overrides for: " + current.defName); }
                    if (current.thingOverrides != null)
                    {
                        ReplaceThings(map, current);
                    }
                    if (current.thingOverrides == null && overrideLogging) { Log.Message("Skipping Thing Overrides for: " + current.defName); }
                }
                else if (overrideLogging) { Log.Message("Current biome not valid for BiomeOverrideDef: " + current.defName); }
            }
        }

        public void ReplaceEdifice(Map map, BiomeOverrideDef current)
        {
            Log.Message("BiomeOverride: Edifice Replacer, Replacing Process: " + current.defName);
            foreach (BiomeOverrideDef.BiomeEdificeOverrides currentEdifice in current.edificeOverrides)
            {
                Log.Message("Replacing: " + currentEdifice.oldEdifice.label + ", with: " + currentEdifice.newEdifice.label);
                foreach (IntVec3 c in map.AllCells)
                {
                    Building edifice = c.GetEdifice(map);
                    if (edifice != null && edifice.def == currentEdifice.oldEdifice)
                    {
                        edifice.Destroy();
                        GenSpawn.Spawn(currentEdifice.newEdifice, c, map);
                    }
                }
            }
        }

        public void ReplaceTerrain(Map map, BiomeOverrideDef current)
        {
            foreach (BiomeOverrideDef.BiomeTerrainOverrides currentTerrain in current.terrainOverrides)
            {
                Log.Message("Replacing: " + currentTerrain.oldTerrain.label + ", with: " + currentTerrain.newTerrain.label);
                foreach (IntVec3 c in map.AllCells)
                {
                    TerrainDef terrain = c.GetTerrain(map);
                    if (terrain == currentTerrain.oldTerrain)
                    {
                        map.terrainGrid.SetTerrain(c, currentTerrain.newTerrain);
                    }
                }
            }
        }

        public void ReplaceThings(Map map, BiomeOverrideDef current)
        {
            foreach (BiomeOverrideDef.BiomeThingOverrides currenThing in current.thingOverrides)
            {
                Log.Message("Replacing: " + currenThing.oldThing.label + ", with: " + currenThing.newThing.label);
                foreach (IntVec3 c in map.AllCells)
                {
                    Thing thing = c.GetFirstThing(map, currenThing.oldThing);
                    if (thing != null)
                    {
                        thing.Destroy();
                        GenSpawn.Spawn(currenThing.newThing, c, map);
                    }
                }
            }
        }
    }
}
