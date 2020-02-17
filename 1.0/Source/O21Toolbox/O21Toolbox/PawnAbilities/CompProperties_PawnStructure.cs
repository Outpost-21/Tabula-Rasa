using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnAbilities
{
    public class CompProperties_PawnStructure : CompProperties
    {
        public CompProperties_PawnStructure()
        {
            this.compClass = typeof(Comp_PawnStructure);
        }

        public string labelString = "Hibernating";

        public string awakenToHostilesIcon = "";
    }
}
