using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class CompProperties_Bondable : CompProperties
    {
        public CompProperties_Bondable()
        {
            this.compClass = typeof(Comp_Bondable);
        }
    }
}
