using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class CompProperties_Spaceship : CompProperties
    {
        // Texture paths and graphics options.
        public List<GraphicPaths> graphicPaths;

        // Roles ship can be used in.
        public List<string> roles;

        // Settings for crew numbers.
        public List<CrewSettings> crewSettings;

        // List of Addons.
        public List<AddonSettings> addonSettings;

        // List of factions that can use the ship.
        public List<string> factions;

        // Tag for PawnGroups handling.
        public string pawnGroupTag;

        public string landingDef;
    }

    public class GraphicPaths
    {
        // Texture used for ship landing or taking off.
        public string landingTexPath;

        // Texture used for ship in flight.
        public string flyingTexPath;

        // Texture used for ship shadow.
        public string shadowTexPath;

        // Draw size of ship
        public Vector2 drawSize;

        // Size of ship.
        public Vector2 size;
    }

    public class CrewSettings
    {
        // Required number of pilots for flight.
        public int pilotReq;

        // Max number of pilots that can fit.
        public int pilotMax;

        //Max number of passengers NOT including pilots.
        public int passengerMax;
    }

    public class AddonSettings
    {
        // Addon Type.
        public string addonType = null;

        // If Type = Turret
        //// Def for Turret.
        public string turretDef = null;

        //// Offset for Turret position.
        public Vector2 turretOffset = new Vector2(0, 0);

        //// When the turret is active (able to fire).
        public string activeWhen = "Always";
    }
}
