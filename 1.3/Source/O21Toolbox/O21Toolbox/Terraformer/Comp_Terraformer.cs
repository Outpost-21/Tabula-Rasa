using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class Comp_Terraformer : ThingComp
    {
        public CompProperties_Terraformer Props => (CompProperties_Terraformer)props;

        public CompPowerTrader powerComp = null;

        public MapComponent_Terraforming mapComp => parent.Map.GetComponent<MapComponent_Terraforming>();

        public int workTick = -50;

        public Thing parentNode = null;

        public int witherTickCurrent = 0;

        //private List<IntVec3> cachedViableCells = new List<IntVec3>();

        private Grid_Terraformer terraformerGrid;

        public Grid_Terraformer TerraformerGrid
        {
            get
            {
                if(terraformerGrid == null)
                {
                    terraformerGrid = mapComp.GetGridByTag(Props.terraformerTag);
                }

                return terraformerGrid;
            }
        }

        public List<IntVec3> GetRadialCells => GenRadial.RadialCellsAround(this.parent.Position, Props.terraformRange, true).ToList();

        public List<IntVec3> GetViableCells
        {
            get
            {
                List<IntVec3> results = new List<IntVec3>();
                if (!TerraformerGrid.terraformerCells.Contains(parent.Position))
                {
                    TerraformerGrid.terraformerCells.Add(parent.Position);
                    TerraformerGrid.MakeDirty(parent.Position);
                }
                if (!TerraformerGrid.growthEdgeGrid.ActiveCells.ToList().NullOrEmpty())
                {
                    results = TerraformerGrid.growthEdgeGrid.ActiveCells.ToList();
                }
                return results;
            }
        }

        //public void RecacheViableCells()
        //{
        //    if (Props.terraformerRules.terrainRules.NullOrEmpty())
        //    {
        //        return;
        //    }

        //    List<IntVec3> allCells = GetRadialCells;
        //    List<IntVec3> potentialCells = new List<IntVec3>();

        //    for (int k = 0; k < allCells.Count(); k++)
        //    {
        //        if (!parent.OccupiedRect().Where(c => c.AdjacentTo8WayOrInside(allCells[k])).EnumerableNullOrEmpty())
        //        {
        //            potentialCells.Add(allCells[k]);
        //        }
        //        else if (GenAdjFast.AdjacentCells8Way(allCells[k]).Where(c => mapComp.terraformedTiles.Where(t => t.Key.AdjacentTo8Way(allCells[k]) && t.Value.terraformer == parent).EnumerableNullOrEmpty()).EnumerableNullOrEmpty())
        //        {
        //            potentialCells.Add(allCells[k]);
        //        }
        //        else if (!GenAdjFast.AdjacentCells8Way(allCells[k]).Where(c => Props.PotentialResults.Contains(c.GetTerrain(parent.Map))).EnumerableNullOrEmpty())
        //        {
        //            potentialCells.Add(allCells[k]);
        //        }
        //    }

        //    for (int i = 0; i < potentialCells.Count(); i++)
        //    {
        //        if (!Props.TerrainCandidates.NullOrEmpty())
        //        {
        //            if (Props.TerrainCandidates.Contains(potentialCells[i].GetTerrain(parent.Map)) && !cachedViableCells.Contains(potentialCells[i]))
        //            {
        //                cachedViableCells.Add(potentialCells[i]);
        //            }
        //        }
        //        else if (!Props.EdificeCandidates.NullOrEmpty())
        //        {
        //            if(potentialCells[i].GetEdifice(parent.Map) != null && Props.EdificeCandidates.Contains(potentialCells[i].GetEdifice(parent.Map).def) && !cachedViableCells.Contains(potentialCells[i]))
        //            {
        //                cachedViableCells.Add(potentialCells[i]);
        //            }
        //        }
        //    }
        //}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            powerComp = parent.TryGetComp<CompPowerTrader>();

            if (!respawningAfterLoad)
            {
                ResetWorkTick();
            }
            parent.Map.GetComponent<MapComponent_Terraforming>().RegisterTerraformer((Building)parent);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

        }

        public void InitializeParams(Thing node)
        {
            parentNode = node;
        }

        public override void CompTick()
        {
            base.CompTick();

            if(((!Props.requiresPower || ((Props.requiresPower && powerComp != null) && powerComp.PowerOn)) && workTick <= 0) && !IsWithering())
            {
                if (Props.successRestores && witherTickCurrent > 0)
                {
                    witherTickCurrent--;
                }
                TryTerraform();
                ResetWorkTick();
            }

            if (IsWithering())
            {
                if(witherTickCurrent >= Props.witherTicks)
                {
                    this.parent.Destroy();
                }
                witherTickCurrent++;                
            }
            else
            {
                if(witherTickCurrent > 0)
                {
                    witherTickCurrent--;
                }
            }

            if (workTick > 0)
            {
                workTick--;
            }

            //HarmRandomPlantInRadius();
        }

        private bool IsWithering()
        {
            if(Props.failsWithoutParent && (parentNode == null || !parentNode.TryGetComp<Comp_Terraformer>().powerComp.PowerOn))
            {
                return true;
            }
            return false;
        }

        private void ResetWorkTick()
        {
            int resultTicks = 0;
            if(Props.terraformTicksMax <= 0)
            {
                resultTicks = Props.terraformTicksExact;
            }
            else
            {
                resultTicks = UnityEngine.Random.Range(Props.terraformTicksMin, Props.terraformTicksMax);
            }

            workTick = resultTicks;
        }

        public override string CompInspectStringExtra()
        {
            return "Work For Current Tile" + ": " + this.workTick.ToString();
        }

        private void TryTerraform()
        {
            //LogUtil.LogMessage("Attempting Terraform");
            TryTerraform_Terrain();
            TryTerraform_Edifice();
            //TryTerraform_Plants();
            //TryTerraform_CreateNodes();
        }

        //private void HarmRandomPlantInRadius()
        //{
        //    IntVec3 c = this.parent.Position + (Rand.InsideUnitCircleVec3 * currentRadiusT).ToIntVec3();
        //    if (!c.InBounds(this.parent.Map))
        //    {
        //        return;
        //    }
        //    if (Props.killUnlistedPlants)
        //    {
        //        Plant plant = c.GetPlant(this.parent.Map);
        //        if(plant != null)
        //        {
        //            if(Props.terraformerRules.plantRules != null)
        //            {
        //                foreach(TerraformerPlantRule rule in Props.terraformerRules.plantRules)
        //                {
        //                    if(!(rule.thingResult.Exists(x => x.thingDef == plant.def) || (rule.thingWhitelist != null && rule.thingWhitelist.Exists(x => x == plant.def))))
        //                    {
        //                        plant.Kill(null, null);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                plant.Kill(null, null);
        //            }
        //        }
        //    }
        //}

        private void TryTerraform_Terrain()
        {
            if(Props.terraformerRules.terrainRules != null)
            {
                List<IntVec3> tileList = GetViableCells;
                List<IntVec3> finalList = new List<IntVec3>();
                if(tileList.Count() > 0)
                {
                    foreach (TerraformerTerrainRule rule in Props.terraformerRules.terrainRules)
                    {
                        for (int k = 0; k < tileList.Count(); k++)
                        {
                            if (!parent.OccupiedRect().Where(c => c.AdjacentTo8WayOrInside(tileList[k])).EnumerableNullOrEmpty())
                            {
                                finalList.Add(tileList[k]);
                            }
                            else if (!GenAdjFast.AdjacentCells8Way(tileList[k]).Where(c => rule.terrainResult.Contains(c.GetTerrain(parent.Map))).EnumerableNullOrEmpty())
                            {
                                finalList.Add(tileList[k]);
                            }
                        }

                        IntVec3 randomTile = finalList.RandomElementByWeightWithDefault(t => GenAdjFast.AdjacentCells8Way(t).Where(c => rule.terrainResult.Contains(c.GetTerrain(parent.Map))).Count(), 0.01f);
                        TerrainDef newTerrain = rule.terrainResult.RandomElement();

                        if (rule.terrainDefsWhitelist.Exists(x => x == randomTile.GetTerrain(parent.Map)) || (rule.terrainDefsWhitelist == null && rule.terrainDefsBlacklist != null && !rule.terrainDefsBlacklist.Exists(x => x == randomTile.GetTerrain(parent.Map))))
                        {
                            TerraformerGrid.MakeDirty(randomTile);
                            parent.Map.terrainGrid.SetTerrain(randomTile, newTerrain);
                            break;
                        }
                    }
                }
            }
        }

        //private List<IntVec3> ListTerraformableTiles(float f)
        //{
        //    List<IntVec3> terraformTiles = new List<IntVec3>();

        //    int num = GenRadial.NumCellsInRadius(this.currentRadiusT - f);
        //    for (int i = 0; i < num; i++)
        //    {
        //        IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
        //        foreach (TerraformerTerrainRule rule in this.Props.terraformerRules.terrainRules)
        //        {
        //            if ((rule.terrainDefsWhitelist != null && rule.terrainDefsWhitelist.Contains(tile.GetTerrain(this.parent.Map))) || (rule.terrainDefsBlacklist != null && rule.terrainDefsBlacklist.Contains(tile.GetTerrain(this.parent.Map))))
        //            {
        //                if ((GenAdjFast.AdjacentCellsCardinal(tile).Any(loc => (bool)(GetSpreaderTerrains(this)?.Contains(loc.GetTerrain(this.parent.Map)))) || ((GenAdjFast.AdjacentCellsCardinal(tile).Any(loc => (bool)(GetSpreaderEdifices(this)?.Contains(loc.GetEdifice(this.parent.Map)?.def)))) || this.currentRadiusT == 0f)) && GenAdjFast.AdjacentCells8Way(tile).Any(loc => !loc.Filled(this.parent.Map)))
        //                {
        //                    terraformTiles.Add(tile);
        //                }
        //            }
        //        }
        //    }

        //    return terraformTiles;
        //}

        private void TryTerraform_Edifice()
        {
            if(Props.terraformerRules.edificeRules != null)
            {
                List<IntVec3> tileList = GetViableCells;
                if (tileList.Count() > 0)
                {
                    foreach (TerraformerThingRule rule in Props.terraformerRules.edificeRules)
                    {
                        if (rule.thingWhitelist.Exists(x => x.IsEdifice()) || (rule.thingWhitelist == null && rule.thingBlacklist != null && !rule.thingBlacklist.Exists(x => x.IsEdifice())))
                        {
                            IntVec3 tile = tileList.RandomElement();
                            tile.GetEdifice(this.parent.Map).Destroy();
                            GenSpawn.Spawn(rule.thingResult.RandomElement(), tile, this.parent.Map);
                            break;
                        }
                    }
                }
            }
        }

        //private List<IntVec3> ListTerraformableEdifices()
        //{
        //    List<IntVec3> terraformTiles = new List<IntVec3>();

        //    int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
        //    for (int i = 0; i < num; i++)
        //    {
        //        IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
        //        foreach (TerraformerThingRule rule in this.Props.terraformerRules.edificeRules)
        //        {
        //            if (((rule.thingWhitelist != null && tile.GetEdifice(this.parent.Map) != null && rule.thingWhitelist.Contains(tile.GetEdifice(this.parent.Map).def)) || (rule.thingBlacklist != null && tile.GetEdifice(this.parent.Map) != null && rule.thingBlacklist.Contains(tile.GetEdifice(this.parent.Map).def))) && GetSpreaderTerrains(this).Contains(tile.GetTerrain(this.parent.Map)))
        //            {
        //                terraformTiles.Add(tile);
        //            }
        //        }
        //    }

        //    return terraformTiles;
        //}

        //public void TryTerraform_Things()
        //{
        //    throw new NotImplementedException();
        //}

        //private void TryTerraform_Plants()
        //{
        //    if (Props.terraformerRules.plantRules.NullOrEmpty())
        //    {
        //        return;
        //    }
        //    foreach (TerraformerPlantRule rule in Props.terraformerRules.plantRules)
        //    {
        //        List<IntVec3> tileList = ListPlantableTiles(rule.ignoreFertility, rule.fertilityAbove, rule.fertilityBelow);
        //        foreach (ThingDefCount tdc in rule.thingResult)
        //        {
        //            if(GetThingsInRadius(tdc.ThingDef).Count < tdc.Count)
        //            {
        //                GenSpawn.Spawn(tdc.ThingDef, tileList.RandomElement(), this.parent.Map);
        //            }
        //        }
        //    }
        //}

        //private List<IntVec3> GetThingsInRadius(ThingDef thingDef)
        //{
        //    List<IntVec3> thingTiles = new List<IntVec3>();

        //    int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
        //    for (int i = 0; i < num; i++)
        //    {
        //        IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
        //        if (tile.GetThingList(this.parent.Map).Any(x => x.def == thingDef))
        //        {
        //            thingTiles.Add(tile);
        //        }
        //    }

        //    return thingTiles;
        //}

        private List<IntVec3> ListPlantableTiles(bool useFertility, float min, float max)
        {
            List<IntVec3> plantableTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.Props.terraformRange);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                if((useFertility || (!useFertility && tile.GetTerrain(this.parent.Map).fertility >= min && tile.GetTerrain(this.parent.Map).fertility <= max)) && Props.PotentialResults.Contains(tile.GetTerrain(this.parent.Map)))
                {
                    plantableTiles.Add(tile);
                }
            }

            return plantableTiles;
        }

        private void TryTerraform_CreateNodes()
        {
            throw new NotImplementedException();
        }

        //public override void PostDrawExtraSelectionOverlays()
        //{
        //    if (this.currentRadiusT < this.Props.terraformRange - 0.0001f)
        //    {
        //        GenDraw.DrawRadiusRing(this.parent.Position, this.currentRadiusT);
        //    }
        //}
    }
}
