using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class MapComponent_Terraforming : MapComponent
    {
        private List<Building> terraformers = new List<Building>();

        private Dictionary<string, Grid_Terraformer> terraformerGrids = new Dictionary<string, Grid_Terraformer>();

        public List<Building> Terraformers
        {
            get
            {
                if (!terraformers.NullOrEmpty())
                {
                    foreach (Building building in terraformers.ToList())
                    {
                        if (building == null || building.Destroyed)
                        {
                            UnregisterTerraformer(building);
                        }
                    }
                }

                return terraformers;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref terraformers, "terraformers", LookMode.Reference);
            Scribe_Collections.Look(ref terraformerGrids, "terraformerGrids", LookMode.Deep);
        }

        public MapComponent_Terraforming(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
        }

        public bool InRangeOfTerraformer(Grid_Terraformer grid, IntVec3 c)
        {
            List<Building> nodes = terraformers.Where(t => t.TryGetComp<Comp_Terraformer>().Props.terraformerTag == terraformerGrids.Where(g => g.Value == grid).First().Key && t.Map == grid.map).ToList();

            if (!nodes.NullOrEmpty())
            {
                foreach (Building building in nodes)
                {
                    Comp_Terraformer comp = building.TryGetComp<Comp_Terraformer>();

                    if (comp.GetRadialCells.Contains(c))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckTerraformers()
        {
        }

        public void RestoreTile(IntVec3 c)
        {
            map.terrainGrid.SetTerrain(c, (map.terrainGrid.UnderTerrainAt(c)));
        }

        public void RegisterTerraformer(Building terraformer)
        {
            this.terraformers.Add(terraformer);
        }

        public void UnregisterTerraformer(Building terraformer)
        {
            this.terraformers.Remove(terraformer);
        }

        public Grid_Terraformer GetGridByTag(string tag)
        {
            Grid_Terraformer result;
            if (!terraformerGrids.EnumerableNullOrEmpty() && terraformerGrids.ContainsKey(tag)) 
            {
                result = terraformerGrids.Where(g => g.Key == tag).First().Value;
            }
            else
            {
                result = GenerateNewGridByTag(tag);
            }
            return result;
        }

        public Grid_Terraformer GenerateNewGridByTag(string tag)
        {
            Grid_Terraformer newGrid = new Grid_Terraformer(map);

            terraformerGrids.Add(tag, newGrid);

            return newGrid;
        }
    }
}
