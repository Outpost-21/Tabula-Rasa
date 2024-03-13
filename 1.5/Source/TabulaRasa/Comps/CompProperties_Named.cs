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
    public class CompProperties_Named : CompProperties
    {
        public CompProperties_Named()
        {
            this.compClass = typeof(Comp_Named);
        }

        public RulePackDef nameMaker;

        public ThingNameFormat nameFormat = ThingNameFormat.Bracketed;
    }
}
