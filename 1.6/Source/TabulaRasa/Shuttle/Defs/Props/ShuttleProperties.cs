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
    public class ShuttleProperties
    {
        public float inFlightScaleFactor = 1f;

        public ShuttleBehaviourProperties behaviour = new ShuttleBehaviourProperties();

        public ShuttleCrewProperties crew = new ShuttleCrewProperties();

        public ShuttleDeathProperties death;

        public List<ShuttleDrawProperties> draw = new List<ShuttleDrawProperties>();

        public ShuttleMotionProperties motion = new ShuttleMotionProperties();

        public ShuttleShadowProperties shadow = new ShuttleShadowProperties();

        public ShuttleTravelProperties travel;

        public ShuttleUpgradeProperties upgrades;
    }
}
