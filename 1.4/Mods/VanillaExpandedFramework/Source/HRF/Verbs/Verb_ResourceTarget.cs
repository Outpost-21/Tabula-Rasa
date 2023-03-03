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
    public class Verb_ResourceTarget : Verb_ResourceBase
    {
        public override bool Available()
        {
            return this.ResourceProps?.TargetResourceSettings?.Count() > 0;
        }
        public override bool TryCastShot()
        {
            base.TryCastShot();
            var targetResourceSettings = this.ResourceProps.TargetResourceSettings;
            if (targetResourceSettings != null)
            {
                foreach (var resourceProperties in targetResourceSettings)
                {
                    if (resourceProperties.hediff != null)
                    {
                        Pawn target = currentTarget.Thing as Pawn;
                        if (target != null)
                        {
                            Utils.AdjustResourceAmount(target, resourceProperties.hediff, resourceProperties.resourcePerUse, resourceProperties.addHediffIfMissing, 
                                resourceProperties, resourceProperties.applyToPart);
                        }
                        if (resourceProperties.effectRadius > 0)
                        {
                            foreach (var cell in Utils.GetAllCellsAround(resourceProperties, new TargetInfo(currentTarget.Cell, this.Caster.Map), CellRect.SingleCell(currentTarget.Cell)))
                            {
                                foreach (var pawn in cell.GetThingList(this.CasterPawn.Map).OfType<Pawn>())
                                {
                                    if (pawn != target)
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
