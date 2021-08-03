using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class CompProperties_Salvage : CompProperties
    {
        public List<ThingDefCountClass> salvageGoods = new List<ThingDefCountClass>();

        public int salvageTime = 0;
    }
}
