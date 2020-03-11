using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Shield
{
    [StaticConstructorOnStartup]
    public class CompProperties_HeatRelease : CompProperties
    {
        public CompProperties_HeatRelease()
        {
            this.compClass = typeof(Comp_HeatRelease);
        }

        public int powerPerDegree = 10;
        public float usefulTemp = 0f;
        public float maxTemp = 10f;
        public bool mustBeIndoors = false;
        public bool mustBeOutdoors = false;
        public bool canBeUnderShield = false;
    }
}
