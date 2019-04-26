using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class Comp_OrbitalRelay : ThingComp
    {
        public CompProperties_OrbitalRelay Props => (CompProperties_OrbitalRelay)this.props;

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
