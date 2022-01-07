using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class CompProperties_Homing : CompProperties
    {
        public CompProperties_Homing()
        {
            this.compClass = typeof(Comp_Homing);
        }

        public int recalculationInterval = 1;

        public float chance = 1f;
    }
}
