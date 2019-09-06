using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class HediffGiver_Racial : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (base.TryApply(pawn, null))
            {
                return;
            }
        }
    }
}
