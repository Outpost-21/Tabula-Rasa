using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class CompProperties_DestroyedLeavings : CompProperties
    {
        public List<ThingDefCountClass> leavings = new List<ThingDefCountClass>();

        public float chance = 1f;

        public FloatRange percentRange = new FloatRange(1f, 1f);

        public bool harvestableOnly = false;

        public CompProperties_DestroyedLeavings()
        {
            compClass = typeof(Comp_DestroyedLeavings);
        }
    }
}
