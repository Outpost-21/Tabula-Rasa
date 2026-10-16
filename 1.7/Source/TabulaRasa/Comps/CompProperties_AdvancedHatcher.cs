using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class CompProperties_AdvancedHatcher : CompProperties
    {
        public CompProperties_AdvancedHatcher()
        {
            compClass = typeof(Comp_AdvancedHatcher);
        }

        public float daysToHatch = 1f;

        public PawnKindDef pawnKind;

        public List<PawnKindDef> pawnKinds = new List<PawnKindDef>();
    }
}
