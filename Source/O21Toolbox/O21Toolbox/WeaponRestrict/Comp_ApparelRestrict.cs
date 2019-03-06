using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponRestrict
{
    public class Comp_ApparelRestrict : ThingComp
    {
        public CompProperties_ApparelRestrict Props
        {
            get
            {
                return (CompProperties_ApparelRestrict)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
