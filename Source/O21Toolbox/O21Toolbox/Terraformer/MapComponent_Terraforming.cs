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
        public List<Building> terraformers = new List<Building>();
        public List<Building> removedTerraformers = new List<Building>();

        public Dictionary<IntVec3, TerraformedInfo> terraformedTiles = new Dictionary<IntVec3, TerraformedInfo>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref terraformers, "terraformers");
            Scribe_Values.Look(ref removedTerraformers, "removedTerraformers");
            Scribe_Collections.Look(ref terraformedTiles, "terraformedTiles");
        }

        public MapComponent_Terraforming(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            CheckTerraformers();
            RestoreAttempt();
        }

        public void CheckTerraformers()
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
            if (Current.Game.tickManager.TicksGame % 1000 == 0 && !removedTerraformers.NullOrEmpty())
            {
                foreach (Building building in removedTerraformers.ToList())
                {
                    if (!terraformedTiles.Any(t => t.Value.terraformer == building))
                    {
                        removedTerraformers.Remove(building);
                    }
                }
            }
        }

        public void RestoreAttempt()
        {
            if(Current.Game.tickManager.TicksGame % 600 == 0)
            {
                if (!removedTerraformers.NullOrEmpty())
                {
                    foreach(Building terraformer in removedTerraformers)
                    {
                        if (terraformedTiles.Any(i => i.Value.terraformer == terraformer))
                        {
                            RestoreTile(terraformedTiles.Where(k => k.Value.terraformer == terraformer).ToList().RandomElement());
                        }
                    }
                }
            }
        }

        public void RestoreTile(KeyValuePair<IntVec3, TerraformedInfo> info)
        {
            if(info.Value.currentTerrain != map.terrainGrid.TerrainAt(info.Key))
            {
                terraformedTiles.Remove(info.Key);
            }
            else
            {
                map.terrainGrid.SetTerrain(info.Key, info.Value.originalTerrain);
            }
        }

        public void RegisterTerraformer(Building terraformer)
        {
            this.terraformers.Add(terraformer);
        }

        public void UnregisterTerraformer(Building terraformer)
        {
            this.terraformers.Remove(terraformer);
            this.removedTerraformers.Add(terraformer);
        }

        public void RegisterTerraformedTile(IntVec3 loc, Building terraformer, TerrainDef original, TerrainDef current)
        {
            if (terraformedTiles.ContainsKey(loc))
            {
                TerraformedInfo oldInfo;
                terraformedTiles.TryGetValue(loc, out oldInfo);
                if(oldInfo != null)
                {
                    terraformedTiles.Remove(loc);
                    terraformedTiles.Add(loc, new TerraformedInfo(terraformer, oldInfo.originalTerrain, current));
                }
            }
            else
            {
                terraformedTiles.Add(loc, new TerraformedInfo(terraformer, original, current));
            }
        }

        public class TerraformedInfo : IExposable
        {
            public Building terraformer;

            public TerrainDef originalTerrain;

            public TerrainDef currentTerrain;

            public TerraformedInfo(Building building, TerrainDef original, TerrainDef current)
            {
                this.terraformer = building;
                this.originalTerrain = original;
                this.currentTerrain = current;
            }

            public void ExposeData()
            {
                Scribe_Values.Look(ref terraformer, "terraformer");
                Scribe_Defs.Look(ref originalTerrain, "originalTerrain");
                Scribe_Defs.Look(ref currentTerrain, "currentTerrain");
            }
        }
    }
}
