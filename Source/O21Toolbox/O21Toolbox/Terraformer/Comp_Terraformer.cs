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
        }

        private void UpdateRadiusT()
        {
            if(currentRadiusT != Props.terraformRange && ListTerraformableTiles().Count <= 0)
            {
                currentRadiusT++;
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
                //Log.Message("Is Withering");
                if(witherTickCurrent >= Props.witherTicks)
                {
                    this.parent.Destroy();
                }
                witherTickCurrent++;                
            }

            if (workTick > 0)
            {
                workTick--;
            }
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

        private void TryTerraform()
        {
            //Log.Message("Attempting Terraform");
            TryTerraform_Terrain();
            //TryTerraform_Plants();
            //TryTerraform_CreateNodes();
        }

        public override string CompInspectStringExtra()
        {
            string text = string.Concat(new string[]
            {
                "Work For Current Tile",
                ": ",
                this.workTick.ToString(),
                "\n",
                "CurrentRadius".Translate().CapitalizeFirst(),
                ": ",
                this.currentRadiusT.ToString(),
                "\n",
                "Max Radius",
                ": ",
                this.Props.terraformRange.ToString()
            });
            return text;
        }

        private void TryTerraform_Terrain()
        {
            List<IntVec3> tileList = ListTerraformableTiles();
            if(tileList.Count() > 0)
            {
                foreach(TerraformerTerrainRule rule in Props.terraformerRules.terrainRules)
                {
                    if(rule.terrainDefsWhitelist.Exists(x => x == tileList.First().GetTerrain(this.parent.Map)) || (rule.terrainDefsWhitelist != null && rule.terrainDefsBlacklist != null && !rule.terrainDefsBlacklist.Exists(x => x == tileList.First().GetTerrain(this.parent.Map))))
                    {
                        this.parent.Map.terrainGrid.SetTerrain(tileList.First(), rule.terrainResult);
                        break;
                    }
                }
            }
        }

        private List<IntVec3> ListTerraformableTiles()
        {
            List<IntVec3> terraformTiles = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(this.currentRadiusT);
            for (int i = 0; i < num; i++)
            {
                IntVec3 tile = this.parent.Position + GenRadial.RadialPattern[i];
                foreach(TerraformerTerrainRule rule in this.Props.terraformerRules.terrainRules)
                {
                    if((rule.terrainDefsWhitelist != null && rule.terrainDefsWhitelist.Contains(tile.GetTerrain(this.parent.Map))) || (rule.terrainDefsBlacklist != null && rule.terrainDefsBlacklist.Contains(tile.GetTerrain(this.parent.Map))))
                    {
                        terraformTiles.Add(tile);
                    }
                }
            }

            //Log.Message("TerraformListed: " + terraformTiles.Count());

            return terraformTiles;
        }

        private void TryTerraform_Plants()
        {
            throw new NotImplementedException();
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
