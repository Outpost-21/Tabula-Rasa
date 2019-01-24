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

        // Type of ship.
        public List<String> typeList;

        // List of factions that can use the ship.
        public List<String> factions;

        public List<CrewSettings> crewSettings;
    }

    public class GraphicData
    {
        // Texture used for ship in flight.
        public string flyingTexPath;

        // Texture used for ship shadow.
        public string shadowTexPath;

        // Texture used for ship landing or taking off.
        public string landingTexPath;

        // Size of ship.
        public Vector3 size;

        // Draw size of ship
        public Vector3 drawSize;
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
}
