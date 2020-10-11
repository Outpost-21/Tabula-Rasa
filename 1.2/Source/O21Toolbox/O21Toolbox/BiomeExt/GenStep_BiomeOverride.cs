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
            IEnumerable<BiomeOverrideDef> enumerable = from def in DefDatabase<BiomeOverrideDef>.AllDefs
                                                       where def.biomeDefs != null
                                                       select def;
            foreach (BiomeOverrideDef current in enumerable)
            {
                if (current.biomeDefs.Contains(map.Biome))
                {
                    if (current.edificeOverrides != null)
                    {
                        ReplaceEdifice(map, current);
                    }
                    if (current.terrainOverrides != null)
                    {
                        ReplaceTerrain(map, current);
                    }
                    if (current.thingOverrides != null)
                    {
                        ReplaceThings(map, current);
                    }
                }
            }
        }

        public void ReplaceEdifice(Map map, BiomeOverrideDef current)
        {
            foreach (BiomeOverrideDef.BiomeEdificeOverrides currentEdifice in current.edificeOverrides)
            {
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
