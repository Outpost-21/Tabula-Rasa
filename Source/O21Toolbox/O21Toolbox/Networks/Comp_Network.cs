using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class Comp_Network : ThingComp
    {
        public CompProperties_Network Props => (CompProperties_Network)this.props;
    }
}
