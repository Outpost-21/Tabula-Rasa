using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelRestrict
{
    public class Comp_BodyRestrict : ThingComp
    {
        public CompProperties_BodyRestrict Props
        {
            get
            {
                return (CompProperties_BodyRestrict)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
