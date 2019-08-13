using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class CompProperties_BodyRestrict : CompProperties
    {
        public CompProperties_BodyRestrict() => this.compClass = typeof(Comp_BodyRestrict);

        public List<BodyTypeDef> BodyDefs = null;
    }
}
