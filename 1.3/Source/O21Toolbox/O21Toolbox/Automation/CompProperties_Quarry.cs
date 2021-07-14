using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Automation
{
    public class CompProperties_Quarry : CompProperties
    {
        public void CompProperties()
        {
            compClass = typeof(Comp_Quarry);
        }

        public MiningSettings defaultMiningSettings;

        public float tickCostMultiplier;

        public float costDebuffPercent;
    }
}
