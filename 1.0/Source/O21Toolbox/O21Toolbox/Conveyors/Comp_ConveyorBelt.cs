using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Conveyors
{
    public class Comp_ConveyorBelt : ThingComp
    {
        public CompProperties_ConveyorBelt Props => (CompProperties_ConveyorBelt)this.props;
    }
}
