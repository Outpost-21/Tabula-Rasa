using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class CompProperties_CustomNetwork_Trader : CompProperties_CustomNetwork
    {
        public CompProperties_CustomNetwork_Trader()
        {
            this.compClass = typeof(Comp_CustomNetwork_Trader);
        }
    }
}
