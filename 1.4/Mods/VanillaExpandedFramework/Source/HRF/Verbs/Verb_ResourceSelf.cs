using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace HRF
{
    public class Verb_ResourceSelf : Verb_ResourceBase
    {
        public override bool Available()
        {
            var targetResourceSettings = this.ResourceProps.TargetResourceSettings;
            if (targetResourceSettings != null)
            {
                foreach (var resourceProperties in targetResourceSettings)
                {
                    var hediffResource = this.CasterPawn.health.hediffSet.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
                    if (hediffResource != null && hediffResource.ResourceAmount == hediffResource.ResourceCapacity)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool TryCastShot()
        {
            base.TryCastShot();
            if (this.CasterPawn != null)
            {
                var targetResourceSettings = this.ResourceProps.TargetResourceSettings;

                if (targetResourceSettings != null)
                {
                    foreach (var resourceProperties in targetResourceSettings)
                    {
                        HRFLog.Message("Giving: " + this.CasterPawn + " - " + resourceProperties.hediff + " - " + resourceProperties.resourcePerUse);
                        if (resourceProperties.affectsAllies || !resourceProperties.affectsAllies && !resourceProperties.affectsEnemies)
                        {
                            Utils.AdjustResourceAmount(this.CasterPawn, resourceProperties.hediff, resourceProperties.resourcePerUse, 
                                resourceProperties.addHediffIfMissing, resourceProperties, resourceProperties.applyToPart);
                        }
                        if (resourceProperties.effectRadius > 0)
                        {
                            foreach (var cell in Utils.GetAllCellsAround(resourceProperties, new TargetInfo(this.CasterPawn.Position, this.CasterPawn.Map), CellRect.SingleCell(this.CasterPawn.Position)))
                            {
                                foreach (var pawn in cell.GetThingList(this.CasterPawn.Map).OfType<Pawn>())
                                {
                                    if (pawn != this.CasterPawn)
                                    {
                                        if (resourceProperties.affectsAllies && (pawn.Faction == this.CasterPawn.Faction || !pawn.Faction.HostileTo(this.CasterPawn.Faction)))
                                        {
                                            Utils.AdjustResourceAmount(pawn, resourceProperties.hediff, resourceProperties.resourcePerUse, resourceProperties.addHediffIfMissing,
                                                resourceProperties, resourceProperties.applyToPart);
                                        }
                                        else if (resourceProperties.affectsEnemies && pawn.Faction.HostileTo(this.CasterPawn.Faction))
                                        {
                                            Utils.AdjustResourceAmount(pawn, resourceProperties.hediff, resourceProperties.resourcePerUse, resourceProperties.addHediffIfMissing,
                                                resourceProperties, resourceProperties.applyToPart);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
