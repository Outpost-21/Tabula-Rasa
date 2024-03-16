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
    public static class NeedsUtil
    {
        public static WorldComp_EnergyNeed GetEnergyNeedWorldComp
        {
            get
            {
                WorldComp_EnergyNeed comp = Find.World.GetComponent(typeof(WorldComp_EnergyNeed)) as WorldComp_EnergyNeed;
                if(comp != null)
                {
                    return comp;
                }
                else
                {
                    LogUtil.Error("Could not find WorldComponent_EnergyNeed.");
                }
                return null;
            }
        }

        public static bool InWirelessChargerRange(this Pawn pawn)
        {
            WorldComp_EnergyNeed comp = GetEnergyNeedWorldComp;
            if (pawn.Spawned && !comp.wirelessChargers.NullOrEmpty())
            {
                List<Building> chargersOnMap = comp.wirelessChargers.Where(wc => wc.Map != null && wc.Map == pawn.Map).ToList();
                if (!chargersOnMap.NullOrEmpty() && chargersOnMap.Any(wc => pawn.Position.DistanceTo(wc.Position) <= wc.def.specialDisplayRadius))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Building> GetLocalChargingSockets(Pawn pawn)
        {
            WorldComp_EnergyNeed comp = GetEnergyNeedWorldComp;
            if(pawn.Spawned && !comp.chargingSockets.NullOrEmpty())
            {
                return comp?.chargingSockets?.Where(wc => wc.Map != null && wc.Map == pawn.Map)?.ToList() ?? new List<Building>();
            }
            return new List<Building>();
        }

        public static Building GetClosestPowerSocket(Pawn pawn)
		{
			Building building = null;
            List<Building> localSockets = GetLocalChargingSockets(pawn);
            if (!localSockets.NullOrEmpty())
            {
                for (int i = 0; i < localSockets.Count(); i++)
                {
                    Building curr = localSockets[i];
                    if ((building == null || building.Position.DistanceTo(pawn.Position) > curr.Position.DistanceTo(pawn.Position)) && building.PowerComp.PowerNet.CurrentStoredEnergy() > 50f)
                    {
                        foreach(IntVec3 cell in GenAdj.CellsAdjacentCardinal(curr).OrderByDescending(selector => selector.DistanceTo(pawn.Position)))
                        {
                            if(cell.Walkable(pawn.Map) && cell.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(cell)) && pawn.CanReach(cell, PathEndMode.OnCell, Danger.Deadly))
                            {
                                building = curr;
                                break;
                            }
                        }
                    }
                }
            }
			return building;
		}
    }
}
