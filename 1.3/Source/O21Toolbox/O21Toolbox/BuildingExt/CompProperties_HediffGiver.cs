using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class CompProperties_HediffGiver : CompProperties
    {
        public CompProperties_HediffGiver()
        {
            compClass = typeof(Comp_HediffGiver);
        }

        public float radius;
        public HediffDef hediffDef;
        public int ticksBeforeApply;
        public int adjustSeverity;
    }
}
