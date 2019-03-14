using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AreaEffects
{
    public class Comp_AreaEffects : ThingComp
    {

        public CompProperties_AreaEffects Props
        {
            get
            {
                return (CompProperties_AreaEffects)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
