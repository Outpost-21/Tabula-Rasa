using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa
{
    //TODO: Remove next RW update
    public class JobGiver_SeekEnergy : ThinkNode_JobGiver
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_SeekEnergy jobGiver = (JobGiver_SeekEnergy)base.DeepCopy(resolve);
            return jobGiver;
        }

        public override float GetPriority(Pawn pawn)
        {
            Need_Energy energy = pawn.needs.TryGetNeed<Need_Energy>();
            if(energy != null && energy.CurCategory >= EnergyCategory.GettingLow)
            {
                return 11.5f;
            }
            return 0f;
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Downed)
            {
                return null;
            }
            Need_Energy energy = pawn.needs.TryGetNeed<Need_Energy>();
            if(energy != null && energy.CurCategory < EnergyCategory.GettingLow)
            {
                return null;
            }
            if(Find.TickManager.TicksGame < GetLastTryTick(pawn) * 2500)
            {
                return null;
            }
            SetLastTryTick(pawn, Find.TickManager.TicksGame);

            // Try power sockets.
            Building building = NeedsUtil.GetClosestPowerSocket(pawn);
            if(building != null)
            {
                foreach (IntVec3 cell in GenAdj.CellsAdjacentCardinal(building).OrderByDescending(selector => selector.DistanceTo(pawn.Position)))
                {
                    if (cell.Walkable(pawn.Map) && cell.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(cell)) && pawn.CanReach(cell, PathEndMode.OnCell, Danger.Deadly))
                    {
                        return new Job(TabulaRasaDefOf.TabulaRasa_RechargeFromSocket, building);
                    }
                }
            }

            // Try consumables.
            // > Held items first first.
            if (pawn.carryTracker is Pawn_CarryTracker carryTracker && carryTracker.CarriedThing is Thing carriedThing && carriedThing.TryGetComp<Comp_EnergySource>() != null)
            {
                if (carriedThing.stackCount > 0)
                {
                    return new Job(TabulaRasaDefOf.TabulaRasa_ConsumeEnergySource, new LocalTargetInfo(carriedThing))
                    {
                        count = carriedThing.stackCount
                    };
                }
            }

            // > Inventory then I guess.
            if (pawn.inventory is Pawn_InventoryTracker inventory && inventory.innerContainer.Any(thing => thing.TryGetComp<Comp_EnergySource>() != null))
            {
                Thing validEnergySource = inventory.innerContainer.FirstOrDefault(thing => thing.TryGetComp<Comp_EnergySource>() != null);
                if (validEnergySource != null)
                {
                    Comp_EnergySource energySourceComp = validEnergySource.TryGetComp<Comp_EnergySource>();

                    int thingCount = (int)Math.Ceiling((energy.MaxLevel - energy.CurLevel) / energySourceComp.Props.energyGiven);
                    thingCount = Math.Min(thingCount, validEnergySource.stackCount);

                    if (thingCount > 0)
                    {
                        return new Job(TabulaRasaDefOf.TabulaRasa_ConsumeEnergySource, new LocalTargetInfo(validEnergySource))
                        {
                            count = thingCount
                        };
                    }
                }
            }

            // > Hmm, alright lets try on the map.
            Thing closestConsumablePowerSource = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, thing => thing.TryGetComp<Comp_EnergySource>() != null && !thing.IsForbidden(pawn) && pawn.CanReserve(new LocalTargetInfo(thing)) && thing.Position.InAllowedArea(pawn) && pawn.CanReach(new LocalTargetInfo(thing), PathEndMode.OnCell, Danger.Deadly));
            if (closestConsumablePowerSource != null)
            {
                Comp_EnergySource energySourceComp = closestConsumablePowerSource.TryGetComp<Comp_EnergySource>();
                if (energySourceComp != null)
                {
                    int thingCount = (int)Math.Ceiling((energy.MaxLevel - energy.CurLevel) / energySourceComp.Props.energyGiven);
                    if (thingCount > 0)
                    {
                        return new Job(TabulaRasaDefOf.TabulaRasa_ConsumeEnergySource, new LocalTargetInfo(closestConsumablePowerSource))
                        {
                            count = thingCount
                        };
                    }
                }
            }

            // > Shit nothing? Yeah you're fucked bud.
            return null;
        }

        private int GetLastTryTick(Pawn pawn)
        {
            int result;
            if (pawn.mindState.thinkData.TryGetValue(base.UniqueSaveKey, out result))
            {
                return result;
            }
            return -99999;
        }

        private void SetLastTryTick(Pawn pawn, int val)
        {
            pawn.mindState.thinkData[base.UniqueSaveKey] = val;
        }
    }
}
