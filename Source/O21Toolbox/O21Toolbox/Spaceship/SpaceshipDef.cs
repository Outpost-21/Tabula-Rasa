using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    /// <summary>
    /// Used to define a new Spaceship.
    /// </summary>
    public class SpaceshipDef : Def
    {
        // Texture paths and graphics options.
        public GraphicPaths graphicPaths;

        // Roles ship can be used in.
        public List<SpaceshipKind> roles;

        // Settings for crew numbers.
        public CrewSettings crewSettings;

        // List of Addons.
        public List<AddonSettings> addonSettings;

        // Crew Options.
        public List<CrewGroupMaker> crewGroupMaker;

        // List of factions that can use the ship.
        public List<FactionDef> factions;

        // Whether or not the ship can do space based tasks.
        public EngineKinds engineType;

        // Size of ship. Also used to determine the minimum size of landing pad.
        public Vector2 size;
    }

    public class GraphicPaths
    {
        // Texture used for ship landing or taking off.
        public string landedTexPath;

        // Texture used for ship landing or taking off.
        public string landingTexPath;

        // Texture used for ship in flight.
        public string flyingTexPath;

        // Texture used for ship shadow.
        public string shadowTexPath;

        // Draw size of ship
        public Vector2 drawSize;
    }

    public class CrewSettings
    {
        // Required number of pilots for flight.
        public bool pilotReq;

        // Max number of pilots that can fit.
        public int pilotMax;

        //Max number of passengers NOT including pilots.
        public int passengerMax;
    }

    public class AddonSettings
    {
        // Addon Type.
        public AddonType addonType;

        // If Type = Turret
        // Def for Turret.
        public ThingDef addonDef;

        // Offset for Turret position.
        public Vector2 addonOffset;

        // When the turret is active (able to fire).
        public ActiveWhen activeWhen;
    }

    public class CrewGroupMaker
    {
        // Type of crew, matches to the spaceship type.
        public SpaceshipKind crewGroupType;

        // List of pawnKinds who can fit in different roles.
        public List<CrewKindOption> crewKindOptions;
    }

    public class CrewKindOption
    {
        // PawnKind for CrewKindOption
        public PawnKindDef pawnKind;

        // CrewRole for CrewKindOption
        public CrewRole crewRole;
    }

    public enum ActiveWhen
    {
        Always,
        Landed,
        Flying
    }

    public enum AddonType
    {
        Building,
        Turret
    }

    public enum CrewRole
    {
        // Used to fly the ship, no pilot means no flying.
        Pilot,
        // Used to control designated weapon systems.
        Gunner,
        // Used to define potential passengers.
        Passenger
    }

    public enum SpaceshipKind
    {
        // Used for traders, periodic and requested.
        Cargo,
        // Used for requesting pawns from a faction.
        Reinforcement,
        // Used in events where they request repairs.
        Damaged,
        // Used when backup or raids drop off troops.
        DispatcherDrop,
        // Used when backup or raids leave the map.
        DispatcherPick,
        // Used to call in airstrikes or airdrops.
        Airstrike
    }

    public enum EngineKinds
    {
        // Capable of in-atmosphere only.
        Atmospheric,
        // Only strong enough to make it to orbital stations.
        Orbital,
        // Only capable of reaching other worlds in the same system.
        Interplanetary,
        // Capable of reaching any other world.
        Hyperdrive
    }
}
