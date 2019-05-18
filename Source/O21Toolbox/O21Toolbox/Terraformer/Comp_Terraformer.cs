using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class Comp_Terraformer : ThingComp
    {
        public CompProperties_Terraformer Props => (CompProperties_Terraformer)this.props;
    }
}
