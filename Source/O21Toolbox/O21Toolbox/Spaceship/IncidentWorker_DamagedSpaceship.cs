using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class IncidentWorker_DamagedSpaceship : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (base.CanFireNowSub(parms) == false)
            {
                return false;
            }
            Map map = (Map)parms.target;
            List<Building_LandingPad> freeLandingPads = Util_LandingPad.GetAllFreeLandingPads(map);
            if (freeLandingPads != null)
            {
                return true;
            }
            if (parms.faction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }
            /** if (GetViableShips(parms).Count > 0)
            {
                return false;
            } **/
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Building_LandingPad> freeLandingPads = Util_LandingPad.GetAllFreeLandingPads(map);
            if (freeLandingPads == null)
            {
                // Should not happen if CanFireNowSub returned true.
                return false;
            }
            Building_LandingPad landingPad = freeLandingPads.RandomElement();
            // Find a random viable ship with viable faction.
            SpaceshipDef spaceshipDef = DefDatabase<SpaceshipDef>.AllDefs.RandomElement();

            // Spawn landing damaged spaceship.
            Spaceship_Landing damagedSpaceship = Util_Spaceship.SpawnLandingSpaceship(landingPad, spaceshipDef, SpaceshipKind.Damaged);
            damagedSpaceship.HitPoints = Mathf.RoundToInt(Rand.Range(0.15f, 0.45f) * damagedSpaceship.HitPoints);
            string letterText = "-- Comlink with " + parms.faction.Name + " --\n\n"
                + "Pilot:\n\n"
                + "\"Hello!\n"
                + "Our ship is damaged and we need some repairs or we won't get very far.\n"
                + "We'll compensate you of course, any help would be appreciated!\n\n"
                + "-- End of transmission --\n\n"
                + "WARNING! Not helping the ship may negatively affect your relations with " + parms.faction.Name + ".";
            Find.LetterStack.ReceiveLetter("Repairs request", letterText, LetterDefOf.NeutralEvent, new TargetInfo(landingPad.Position, landingPad.Map));
            return true;
        }

        protected IEnumerable<Faction> CandidateFactions(Map map, bool desperate = false)
        {
            return from f in Find.FactionManager.AllFactions
                   where this.FactionCanBeShipSource(f, map, desperate)
                   select f;
        }

        protected virtual bool FactionCanBeShipSource(Faction f, Map map, bool desperate = false)
        {
            return !f.IsPlayer && !f.defeated && (desperate || (f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp) && f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.SeasonalTemp)));
        }
    }
}
