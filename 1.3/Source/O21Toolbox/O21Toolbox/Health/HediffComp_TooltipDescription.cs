using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Health
{
    public class HediffComp_TooltipDescription : HediffComp
    {
        public override string CompTipStringExtra => parent.def.description;
    }
}
