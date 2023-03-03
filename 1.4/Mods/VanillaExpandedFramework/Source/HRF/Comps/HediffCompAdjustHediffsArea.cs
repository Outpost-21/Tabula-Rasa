using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    public class HediffCompProperties_AdjustHediffsArea : HediffCompProperties_AdjustHediffs
    {
        public bool stackEffects;
        public int stackMax = -1;
        public HediffCompProperties_AdjustHediffsArea()
        {
            this.compClass = typeof(HediffCompAdjustHediffsArea);
        }
    }

    [HotSwappable]
    public class HediffCompAdjustHediffsArea : HediffComp_AdjustHediffs, IAdjustResouceInArea
    {
        public new HediffCompProperties_AdjustHediffsArea Props => this.props as HediffCompProperties_AdjustHediffsArea;
        public override void ResourceTick()
        {
            if (Active)
            {
                foreach (var option in Props.resourceSettings)
                {
                    var num = option.GetResourceGain(this);
                    var affectedCells = Utils.GetAllCellsAround(option, this.Pawn, this.Pawn.OccupiedRect());
                    foreach (var cell in affectedCells)
                    {
                        foreach (var pawn in cell.GetThingList(this.Pawn.Map).OfType<Pawn>())
                        {
                            if (pawn == this.Pawn && !option.affectsSelf) continue;

                            if (option.affectsAllies && (pawn.Faction == this.Pawn.Faction || !pawn.Faction.HostileTo(this.Pawn.Faction)))
                            {
                                AppendResource(pawn, option, num);
                            }
                            else if (option.affectsEnemies && pawn.Faction.HostileTo(this.Pawn.Faction))
                            {
                                AppendResource(pawn, option, num);
                            }
                        }
                    }
                }
            }
        }

        public bool Active => this.Pawn.Map != null;

        public bool InRadiusFor(IntVec3 cell, HediffResourceDef hediffResourceDef)
        {
            if (Active)
            {
                var option = GetFirstResourcePropertiesFor(hediffResourceDef);
                if (option != null && cell.DistanceTo(this.Pawn.Position) <= option.effectRadius)
                {
                    return true;
                }
            }
            return false;
        }
        public void AppendResource(Pawn pawn, ResourceProperties resourceProperties, float num)
        {
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
                        if (amplifiers.Any(x => x != this))
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
                    hediffResource.TryAddAmplifier(this);
                }
            }
        }

        public float GetResourceCapacityGainFor(HediffResourceDef hediffResourceDef)
        {
            if (Active)
            {
                var option = GetFirstResourcePropertiesFor(hediffResourceDef);
                return option.maxResourceCapacityOffset;
            }
            return 0f;
        }
        
        public ResourceProperties GetFirstResourcePropertiesFor(HediffResourceDef hediffResourceDef)
        {
            return this.Props.resourceSettings.FirstOrDefault(x => x.hediff == hediffResourceDef);
        }

        
    }
}
