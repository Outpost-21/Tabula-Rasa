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

        public float tickCostMultiplier = 1f;

        public float costDebuffPercent = 0.20f;

        public int maxDebuffCount = 4;

        public float outputCountMultiplier = 1f;
    }
}
