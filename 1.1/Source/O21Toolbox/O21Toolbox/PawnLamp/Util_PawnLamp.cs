using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnLamp
{
    public static class Util_PawnLamp
    {
        public static ThingDef PawnLampDef
        {
            get
            {
                return ThingDef.Named("PawnLamp");
            }
        }
    }
}
