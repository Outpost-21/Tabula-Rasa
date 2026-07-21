using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleMotionProperties
    {
        public float fuelConsumption = 0f;

        public int collisionCheckDistance = 5;

        public int moveWarmupTick = 60;

        public bool doTurnWhenMoving = true;

        public bool needsRunway = false;

        public int runwayLength = 2;

        public int runwayTickMax = 60;

        public float moveSpeed = 0.15f;

        public float turningSpeed = 0.08f;

        public float flyingAngleChange = 1f;

        public float turningAngleChange = 2f;

        public FlightMode defaultFlightMode = FlightMode.None;

        public bool canToggleFlightMode = false;

        public FlightMode defaultLandingMode = FlightMode.None;

        public bool canToggleLandingMode = false;

        public string toggleMovementLabel = "TabulaRasa.ToggleMovementLabel";

        public string toggleMovementOffLabel = "TabulaRasa.ToggleMovementOffLabel";

        public string toggleMovementDesc = "TabulaRasa.ToggleMovementDesc";

        public string toggleMovementOffDesc = "TabulaRasa.ToggleMovementOffDesc";

        public string toggleMovementIcon = "TabulaRasa/UI/Shuttles/Hover";

        public string toggleMovementOffIcon = "TabulaRasa/UI/Shuttles/Flight";

        public string takeoffLabel = "TabulaRasa.ShuttleTakeOffLabel";

        public string takeoffDesc = "TabulaRasa.ShuttleTakeOffDesc";

        public string takeOffIcon = "TabulaRasa/UI/Shuttles/Up";

        public string landLabel = "TabulaRasa.ShuttleLandLabel";

        public string landDesc = "TabulaRasa.ShuttleLandDesc";

        public string landIcon = "TabulaRasa/UI/Shuttles/Down";
    }
}
