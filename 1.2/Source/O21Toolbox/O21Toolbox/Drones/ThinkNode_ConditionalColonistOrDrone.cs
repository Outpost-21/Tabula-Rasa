using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Drones
{
    public class ThinkNode_ConditionalColonistOrDrone : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.IsColonist || pawn.IsPlayerControlledDrone();
        }
    }
}
