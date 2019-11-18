using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class CompProperties_PawnSpawner : CompProperties
    {
        public CompProperties_PawnSpawner()
        {
            this.compClass = typeof(Comp_PawnSpawner);
        }
        
        public PawnKindDef pawnKind;
    }
}
