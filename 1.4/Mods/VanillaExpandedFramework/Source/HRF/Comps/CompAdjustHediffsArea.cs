using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class CompProperties_AdjustHediffsArea : CompProperties_AdjustHediffs
    {
        public bool stackEffects;
        public int stackMax = -1;
        public CompProperties_AdjustHediffsArea()
        {
            this.compClass = typeof(CompAdjustHediffsArea);
        }
    }

    public class CompAdjustHediffsArea : CompAdjustHediffs, IAdjustResouceInArea
    {
        private CompPowerTrader powerComp;
        private CompRefuelable fuelComp;
        private CompFlickable flickableComp;

        public new CompProperties_AdjustHediffsArea Props => this.props as CompProperties_AdjustHediffsArea;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = this.parent.GetComp<CompPowerTrader>();
            fuelComp = this.parent.GetComp<CompRefuelable>();
            flickableComp = this.parent.GetComp<CompFlickable>();
        }
        public override void ResourceTick()
        {
            HRFLog.Message("Active: " + Active + " - " + this);
            HRFLog.Message("this.parent.Map: " + this.parent.Map);
            HRFLog.Message(" IsEnabled(): " + IsEnabled());
            if (Active)
            {
                foreach (var option in Props.resourceSettings)
                {
                    var num = Utils.GetResourceGain(option, this);
                    var affectedCells = Utils.GetAllCellsAround(option, this.parent, this.parent.OccupiedRect());
                    foreach (var cell in affectedCells)
                    {
                        foreach (var pawn in cell.GetThingList(this.parent.Map).OfType<Pawn>())
                        {
                            if (pawn == this.parent && !option.affectsSelf) continue;

                            if (option.affectsAllies && (pawn.Faction == this.parent.Faction || !pawn.Faction.HostileTo(this.parent.Faction)))
                            {
                                AppendResource(pawn, option, num);
                            }
                            else if (option.affectsEnemies && pawn.Faction.HostileTo(this.parent.Faction))
                            {
                                AppendResource(pawn, option, num);
                            }
                        }
                    }
                }
            }
        }

        public bool Active => this.parent.Map != null && IsEnabled();
        public override Pawn PawnHost => null;
        public bool IsEnabled()
        {
            if (flickableComp != null && !flickableComp.SwitchIsOn)
            {
                return false;
            }
            if (powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }
            if (fuelComp != null && !fuelComp.HasFuel)
            {
                return false;
            }
            return true;
        }
        public bool InRadiusFor(IntVec3 cell, HediffResourceDef hediffResourceDef)
        {
            if (Active)
            {
                var option = GetFirstResourcePropertiesFor(hediffResourceDef);
                if (option != null && cell.DistanceTo(this.parent.Position) <= option.effectRadius)
                {
                    return true;
                }
            }
            return false;
        }
        public void AppendResource(Pawn pawn, ResourceProperties resourceProperties, float num)
        {
            HRFLog.Message("AppendResource: " + pawn);
            var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
            if (hediffResource != null && !hediffResource.CanGainResource)
            {
                return;
            }
            else
            {
                if (hediffResource != null)
                {
                    var amplifiers = hediffResource.GetAmplifiersFor(resourceProperties.hediff);
                    if (!this.Props.stackEffects)
                    {
                        if (amplifiers.Count() > 0 && amplifiers.Any(x => x != this))
                        {
                            return;
                        }
                    }
                    if (this.Props.stackMax != -1)
                    {
                        if (amplifiers.Count() >= this.Props.stackMax && !amplifiers.Contains(this))
                        {
                            return;
                        }
                    }
                }
                hediffResource = Utils.AdjustResourceAmount(pawn, resourceProperties.hediff, num, resourceProperties.addHediffIfMissing, resourceProperties, resourceProperties.applyToPart);
                if (hediffResource != null)
                {
                    HRFLog.Message(this.parent + " is affecting " + pawn + " - " + resourceProperties.hediff);
                    hediffResource.TryAddAmplifier(this);
                }
            }
        }

        public float GetResourceCapacityGainFor(HediffResourceDef hediffResourceDef)
        {
            if (Active)
            {
                return this.GetCapacityFor(GetFirstResourcePropertiesFor(hediffResourceDef));
            }
            return 0f;
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (Active)
            {
                foreach (var option in this.Props.resourceSettings)
                {
                    GenDraw.DrawFieldEdges(Utils.GetAllCellsAround(option, this.parent, this.parent.OccupiedRect()).ToList());
                }
            }
        }

        public ResourceProperties GetFirstResourcePropertiesFor(HediffResourceDef hediffResourceDef)
        {
            return this.Props.resourceSettings.FirstOrDefault(x => x.hediff == hediffResourceDef);
        }
    }
}
