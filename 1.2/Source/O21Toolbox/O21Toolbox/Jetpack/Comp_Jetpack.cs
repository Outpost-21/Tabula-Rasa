using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Jetpack
{
    public class Comp_Jetpack : ThingComp
    {
        public CompProperties_Jetpack Props => (CompProperties_Jetpack)props;

        public int remainingCharges = 0;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref remainingCharges, "remainingCharges");
        }

        public bool CanBeUsed => remainingCharges > 0;

        public void UsedOnce()
        {
            remainingCharges--;
        }

        public void Refuel(int count)
        {
            if(count >= Props.maxCharges)
            {
                remainingCharges = Props.maxCharges;
            }
            else
            {
                remainingCharges = count;
            }
        }
	}
}
