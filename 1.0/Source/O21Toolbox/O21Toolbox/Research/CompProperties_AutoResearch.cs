using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Research
{
    public class CompProperties_AutoResearch : CompProperties
    {
        public CompProperties_AutoResearch()
        {
            this.compClass = typeof(Comp_AutoResearch);
        }

        public bool requiresPower = false;

        public float researchSpeedFactor = 1.0f;

        public PawnKindDef pawnKind = null;
    }
}
