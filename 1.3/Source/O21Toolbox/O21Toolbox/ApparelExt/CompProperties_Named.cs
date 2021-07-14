using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
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

    public enum ThingNameFormat
    {
        Prefix,
        Suffix,
        Bracketed,
        Replace
    }
}
