using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;
using Verse;
using Verse.Noise;
using RimWorld;
using RimWorld.Planet;

namespace O21Toolbox.Spaceship
{
    public static class Util_OrbitalRelay
    {
        public static Building_OrbitalRelay GetOrbitalRelay(Map map)
        {
            if (map.listerBuildings.AllBuildingsColonistOfClass<Building_OrbitalRelay>() == null)
            {
                return null;
            }
            return map.listerBuildings.AllBuildingsColonistOfClass<Building_OrbitalRelay>().First() as Building_OrbitalRelay;
        }

        public static void TryUpdateLandingPadAvailability(Map map)
        {
            Building_OrbitalRelay orbitalRelay = GetOrbitalRelay(map);
            if (orbitalRelay != null)
            {
                orbitalRelay.UpdateLandingPadAvailability();
            }
        }

    }
}
