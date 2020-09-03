using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class CompProperties_Resurrection : CompProperties
    {
        public CompProperties_Resurrection()
        {
            this.compClass = typeof(Comp_Resurrection);
        }

        public IntRange ticksToResurrect = new IntRange(0, 0);

        public float chanceToResurrect = 1f;

        public BodyPartDef requiredBodyPart;
    }
}
