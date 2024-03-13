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
    public class CompProperties_Mining : CompProperties
    {
        public CompProperties_Mining()
        {
            compClass = typeof(Comp_Mining);
        }

        public MiningSettings defaultMiningSettings;

        public float tickCostMultiplier;

        public float costDebuffPercent;
    }
}
