using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ResearchBenchSub
{
    public class CompProperties_ResearchBenchSubstitutes : CompProperties
    {
        public CompProperties_ResearchBenchSubstitutes()
        {
            this.compClass = typeof(Comp_ResearchBenchSubstitutes);
        }

        public List<ThingDef> ActLikeResearchBench = null;

        public List<ThingDef> ActLikeResearchFacility = null;
    }
}
