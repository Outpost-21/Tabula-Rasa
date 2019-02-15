using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Converter
{
    public class Comp_Converter : ThingComp
    {
        public CompProperties_Converter Props
        {
            get
            {
                return (CompProperties_Converter)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
