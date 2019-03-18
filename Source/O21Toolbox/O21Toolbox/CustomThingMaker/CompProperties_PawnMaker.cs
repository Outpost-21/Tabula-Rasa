using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomThingMaker
{
    public class CompProperties_PawnMaker : CompProperties
    {
        public CompProperties_PawnMaker()
        {
            this.compClass = typeof(Comp_PawnMaker);
        }
        
        public PawnKindDef Pawnkind;
    }
}
