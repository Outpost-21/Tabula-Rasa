﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.AI;    // Needed when you do something with the AI
//using Verse.Sound;   // Needed when you do something with the Sound

namespace O21Toolbox.Spaceship
{
    public class IncidentWorker_DispatcherDrop : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (base.CanFireNowSub(parms) == false)
            {
                return false;
            }
            if (Util_Faction.MiningCoFaction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }
            Map map = (Map)parms.target;
            if (Expedition.IsWeatherValidForExpedition(map))
            {
                Building_OrbitalRelay orbitalRelay = Util_OrbitalRelay.GetOrbitalRelay(map);
                if ((orbitalRelay != null)
                    && (orbitalRelay.powerComp.PowerOn))
                {
                    Building_LandingPad landingPad = Util_LandingPad.GetBestAvailableLandingPadReachingMapEdge(map);
                    if (landingPad != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Building_LandingPad landingPad = Util_LandingPad.GetBestAvailableLandingPadReachingMapEdge(map);
            if (landingPad == null)
            {
                // Should not happen if CanFireNowSub returned true.
                return false;
            }

            // Spawn landing dispatcher spaceship.
            FlyingSpaceshipLanding dispatcherSpaceship = Util_Spaceship.SpawnLandingSpaceship(landingPad, SpaceshipKind.DispatcherDrop);
            return true;
        }
    }
}
