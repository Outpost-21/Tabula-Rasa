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
    public class CompProperties_SelfErase : CompProperties
    {
        public CompProperties_SelfErase() => this.compClass = typeof(Comp_SelfErase);
    }
}
