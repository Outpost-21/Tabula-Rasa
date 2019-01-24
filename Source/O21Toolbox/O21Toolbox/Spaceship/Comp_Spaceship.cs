using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class Comp_Spaceship : ThingComp
    {
        public CompProperties_Spaceship Props
        {
            get
            {
                return (CompProperties_Spaceship)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
