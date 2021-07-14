using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Jetpack
{
    public class CompProperties_Jetpack : CompProperties
    {
        public CompProperties_Jetpack()
        {
            compClass = typeof(Comp_Jetpack);
        }

        public FloatRange jumpRange = new FloatRange(5f, 20f);
        public ThingDef skyfallerDef;
        public int cooldownTicks = 0;

        public int maxCharges = 1;
        public ThingDef fuelDef;
        public int fuelCountToRefill = 1;
        public int baseRefuelTicks = 60;
        public SoundDef soundRefuel;
        public string chargeNoun = "Charge";
        public bool displayGizmoUndrafted = true;
        public bool displayGizmoDrafted = true;
    }
}
