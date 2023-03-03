using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
    public class WorkGiver_MaintainResourceBuildings : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            if (CompMaintainableResourceBuilding.maintainables.TryGetValue(pawn.Map, out var maintainables))
            {
                return maintainables.Where(x => x.Faction == pawn.Faction);
            }
            return null;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (CompMaintainableResourceBuilding.maintainables.TryGetValue(pawn.Map, out var maintainables))
            {
                return maintainables.Where(x => x.Faction == pawn.Faction).Count() == 0;
            }
            return false;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building building = t as Building;
            if (building == null)
            {
                return false;
            }
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            if (!t.def.useHitPoints || t.HitPoints == t.MaxHitPoints)
            {
                return false;
            }
            if (pawn.Faction == Faction.OfPlayerSilentFail && !pawn.Map.areaManager.Home[t.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
                return false;
            }

            if (!pawn.CanReserve(building, 1, -1, null, forced))
            {
                return false;
            }
            if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }
            if (building.def.mineable && building.Map.designationManager.DesignationAt(building.Position, DesignationDefOf.Mine) != null)
            {
                return false;
            }
            CompMaintainableResourceBuilding compMaintainable = building.TryGetComp<CompMaintainableResourceBuilding>();
            if (compMaintainable.CurStage == MaintainableStage.Healthy)
            {
                return false;
            }
            if (!compMaintainable.CanMaintain(pawn, out string failReason))
            {
                JobFailReason.Is(failReason);
                return false;
            }
            if (building.IsBurning())
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("ART_Maintain"), t);
        }
    }
}