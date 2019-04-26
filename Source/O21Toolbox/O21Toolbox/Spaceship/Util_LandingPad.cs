using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class Util_LandingPad
    {
        public static Building_LandingPad GetBestAvailableLandingPad(Map map)
        {
            List<Building_LandingPad> allAvailableLandingPads = GetAllFreeAndPoweredLandingPads(map);
            if(allAvailableLandingPads == null)
            {
                return null;
            }
            foreach (Building_LandingPad landingPad in allAvailableLandingPads)
            {
                if (landingPad.isPrimary)
                {
                    return landingPad;
                }
            }
            return allAvailableLandingPads.RandomElement();
        }

        /** public static Building_LandingPad GetBestAvailableLandingPadReachingMapEdge(Map map)
        {
            IntVec3 exitSpot = IntVec3.Invalid;
            // Check pawns can reach map edge from best landing pad.
            Building_LandingPad bestLandingPad = GetBestAvailableLandingPad(map);
            if (bestLandingPad != null)
            {
                if (Expedition.TryFindRandomExitSpot(map, bestLandingPad.Position, out exitSpot))
                {
                    return bestLandingPad;
                }
            }
            // Check pawns can exit map from any landing pad.
            List<Building_LandingPad> allAvailableLandingPads = Util_LandingPad.GetAllFreeAndPoweredLandingPads(map);
            if (allAvailableLandingPads != null)
            {
                foreach (Building_LandingPad landingPad in allAvailableLandingPads.InRandomOrder())
                {
                    if (Expedition.TryFindRandomExitSpot(map, landingPad.Position, out exitSpot))
                    {
                        return landingPad;
                    }
                }
            }
            return null;
        } **/

        // Return a list of all free and powered landing pads.
        public static List<Building_LandingPad> GetAllFreeAndPoweredLandingPads(Map map)
        {
            if (map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>() == null)
            {
                // No landing pad on the map.
                return null;
            }
            List<Building_LandingPad> allFreeAndPoweredLandingPads = new List<Building_LandingPad>();
            foreach (Building building in map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPad landingPad = building as Building_LandingPad;
                if (landingPad.isFreeAndPowered)
                {
                    allFreeAndPoweredLandingPads.Add(landingPad);
                }
            }
            if (allFreeAndPoweredLandingPads.Count > 0)
            {
                return allFreeAndPoweredLandingPads;
            }
            return null;
        }

        // Return a list of all free landing pads.
        public static List<Building_LandingPad> GetAllFreeLandingPads(Map map)
        {
            if (map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>() == null)
            {
                // No landing pad on the map.
                return null;
            }
            List<Building_LandingPad> allFreeLandingPads = new List<Building_LandingPad>();
            foreach (Building building in map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPad landingPad = building as Building_LandingPad;
                if (landingPad.isFree)
                {
                    allFreeLandingPads.Add(landingPad);
                }
            }
            if (allFreeLandingPads.Count > 0)
            {
                return allFreeLandingPads;
            }
            return null;
        }
    }
}
