using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Bunker
{
    public class CompProperties_Bunker : CompProperties
    {
        public CompProperties_Bunker()
        {
            this.compClass = typeof(Comp_Bunker);
        }

        public int pawnCapacity = 1;
    }
}
