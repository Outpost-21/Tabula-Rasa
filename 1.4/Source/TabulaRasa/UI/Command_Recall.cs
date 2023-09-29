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
    public class Command_Recall : Command_Action
    {
        public Thing thing;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => thing.TryGetComp<Comp_Recall>().DestinationFloatMenuOptions(true);
    }
}
