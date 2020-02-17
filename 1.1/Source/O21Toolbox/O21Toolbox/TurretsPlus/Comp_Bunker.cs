using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.TurretsPlus
{
    public class Comp_Bunker : ThingComp
    {
        public CompProperties_Bunker Props
        {
            get
            {
                return (CompProperties_Bunker)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
