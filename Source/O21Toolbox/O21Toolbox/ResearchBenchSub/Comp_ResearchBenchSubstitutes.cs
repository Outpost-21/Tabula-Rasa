using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ResearchBenchSub
{
    public class Comp_ResearchBenchSubstitutes : ThingComp
    {
        /// <summary>
        /// Properties for this Comp.
        /// </summary>
        public CompProperties_ResearchBenchSubstitutes Props
        {
            get
            {
                return props as CompProperties_ResearchBenchSubstitutes;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
