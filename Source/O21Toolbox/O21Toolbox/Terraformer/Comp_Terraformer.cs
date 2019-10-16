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
        public CompProperties_Terraformer Props => (CompProperties_Terraformer)this.props;

        public CompPowerTrader powerComp = null;

        public int workTick = -50;

        public Thing parentNode = null;

        public int witherTickCurrent = 0;

        public float currentRadiusT = 0f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            powerComp = this.parent.TryGetComp<CompPowerTrader>();

            if (!respawningAfterLoad)
            {
                UpdateRadiusT();
                ResetWorkTick();
            }
            if (this.Props.canNatureRestore)
            {
                this.parent.Map.GetComponent<MapComponent_Terraforming>().RegisterTerraformer((Building)this.parent);
            }
        }

        private void UpdateRadiusT()
        {
            if((currentRadiusT - Props.terraformRangeGap) != Props.terraformRange && ListTerraformableTiles(Props.terraformRangeGap).Count <= 0)
            {
                currentRadiusT++;
                if(currentRadiusT > Props.terraformRange)
                {
                    currentRadiusT = Props.terraformRange;
                }
            }
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
                UpdateRadiusT();
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
            string text = string.Concat(new string[]
            {
                "Work For Current Tile".Translate().CapitalizeFirst(),
                ": ",
                this.workTick.ToString(),
                "\n",
                "CurrentRadius".Translate().CapitalizeFirst(),
                ": ",
                this.currentRadiusT.ToString(),
                "\n",
                "Max Radius".Translate().CapitalizeFirst(),
                ": ",
                this.Props.terraformRange.ToString(),
            });
            return text;
        }

        private void TryTerraform()
        {
            //Log.Message("Attempting Terraform");
            TryTerraform_Terrain();
            TryTerraform_Edifice();
            if (currentRadiusT >= Props.terraformRange)
            {
                TryTerraform_Plants();
            }
            //TryTerraform_CreateNodes();
        }

        private void HarmRandomPlantInRadius()
        {
            IntVec3 c = this.parent.Position + (Rand.InsideUnitCircleVec3 * currentRadiusT).ToIntVec3();
            if (!c.InBounds(this.parent.Map))
            {
                return;
            }
            if (Props.killUnlistedPlants)
            {
                Plant plant = c.GetPlant(this.parent.Map);
                if(plant != null)
                {
                    if(Props.terraformerRules.plantRules != null)
                    {
                        foreach(TerraformerPlantRule rule in Props.terraformerRules.plantRules)
                        {
                            if(!(rule.thingResult.Exists(x => x.thingDef == plant.def) || (rule.thingWhitelist != null && rule.thingWhitelist.Exists(x => x == plant.def))))
                            {
                                plant.Kill(null, null);
                            }
                        }
                    }
                    else
                    {
                        plant.Kill(null, null);
                    }
                }
            }
        }

        private void TryTerraform_Terrain()
        {
            if(Props.terraformerRules.terrainRules != null)
            {
                List<IntVec3> tileList = ListTerraformableTiles(0);
                if(tileList.Count() > 0)
                {
                    foreach(TerraformerTerrainRule rule in Props.terraformerRules.terrainRules)
                    {
                        IntVec3 randomTile = tileList.RandomElement();
                        TerrainDef newTerrain = rule.terrainResult.RandomElement();
                        TerrainDef oldTerrain = randomTile.GetTerrain(this.parent.Map);
                        if (rule.terrainDefsWhitelist.Exists(x => x == randomTile.GetTerrain(this.parent.Map)) || (rule.terrainDefsWhitelist == null && rule.terrainDefsBlacklist != null && !rule.terrainDefsBlacklist.Exists(x => x == randomTile.GetTerrain(this.parent.Map))))
                        {
                            this.parent.Map.terrainGrid.SetTerrain(randomTile, newTerrain);
                            this.parent.Map.GetComponent<MapComponent_Terraforming>().RegisterTerraformedTile(randomTile, (Building)this.parent, oldTerrain, newTerrain);
                            break;
                        }
                    }
                }
            }
        }

        private List<IntVec3> ListTerraformableTiles(float f)
        {
            List<IntVec3> terraformTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.currentRadiusT - f);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                foreach (TerraformerTerrainRule rule in this.Props.terraformerRules.terrainRules)
                {
                    if ((rule.terrainDefsWhitelist != null && rule.terrainDefsWhitelist.Contains(tile.GetTerrain(this.parent.Map))) || (rule.terrainDefsBlacklist != null && rule.terrainDefsBlacklist.Contains(tile.GetTerrain(this.parent.Map))))
                    {
                        terraformTiles.Add(tile);
                    }
                }
            }

            //Log.Message("TerraformListed: " + terraformTiles.Count());

            return terraformTiles;
        }

        private void TryTerraform_Edifice()
        {
            if(Props.terraformerRules.edificeRules != null)
            {
                List<IntVec3> tileList = ListTerraformableEdifices();
                if (tileList.Count() > 0)
                {
                    foreach (TerraformerThingRule rule in Props.terraformerRules.edificeRules)
                    {
                        if (rule.thingWhitelist.Exists(x => x.IsEdifice()) || (rule.thingWhitelist == null && rule.thingBlacklist != null && !rule.thingBlacklist.Exists(x => x.IsEdifice())))
                        {
                            IntVec3 tile = tileList.First();
                            tile.GetEdifice(this.parent.Map).Destroy();
                            GenSpawn.Spawn(rule.thingResult.RandomElement(), tile, this.parent.Map);
                            break;
                        }
                    }
                }
            }
        }

        private List<IntVec3> ListTerraformableEdifices()
        {
            List<IntVec3> terraformTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                foreach (TerraformerThingRule rule in this.Props.terraformerRules.edificeRules)
                {
                    if ((rule.thingWhitelist != null && tile.GetEdifice(this.parent.Map) != null && rule.thingWhitelist.Contains(tile.GetEdifice(this.parent.Map).def)) || (rule.thingBlacklist != null && tile.GetEdifice(this.parent.Map) != null && rule.thingBlacklist.Contains(tile.GetEdifice(this.parent.Map).def)))
                    {
                        terraformTiles.Add(tile);
                    }
                }
            }

            return terraformTiles;
        }

        public void TryTerraform_Things()
        {
            throw new NotImplementedException();
        }

        private void TryTerraform_Plants()
        {
            foreach (TerraformerPlantRule rule in Props.terraformerRules.plantRules)
            {
                List<IntVec3> tileList = ListPlantableTiles(rule.ignoreFertility, rule.fertilityAbove, rule.fertilityBelow);
                foreach (ThingDefCount tdc in rule.thingResult)
                {
                    if(GetThingsInRadius(tdc.ThingDef).Count < tdc.Count)
                    {
                        GenSpawn.Spawn(tdc.ThingDef, tileList.RandomElement(), this.parent.Map);
                    }
                }
            }
        }

        private List<IntVec3> GetThingsInRadius(ThingDef thingDef)
        {
            List<IntVec3> thingTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                if (tile.GetThingList(this.parent.Map).Any(x => x.def == thingDef))
                {
                    thingTiles.Add(tile);
                }
            }

            return thingTiles;
        }

        private List<IntVec3> ListPlantableTiles(bool useFertility, float min, float max)
        {
            List<IntVec3> plantableTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                if(useFertility || (!useFertility && tile.GetTerrain(this.parent.Map).fertility >= min && tile.GetTerrain(this.parent.Map).fertility <= max))
                plantableTiles.Add(tile);
            }

            return plantableTiles;
        }

        private void TryTerraform_CreateNodes()
        {
            throw new NotImplementedException();
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (this.currentRadiusT < this.Props.terraformRange - 0.0001f)
            {
                GenDraw.DrawRadiusRing(this.parent.Position, this.currentRadiusT);
            }
        }
    }
}
