using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    public static class Utility_MechanicalPawn
    {
        public static bool IsMechanical(this Pawn pawn)
        {
            return pawn.def.HasModExtension<MechanicalPawnProperties>();
        }
    }
}
