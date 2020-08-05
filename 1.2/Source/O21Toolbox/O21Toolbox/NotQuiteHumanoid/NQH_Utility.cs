using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.NotQuiteHumanoid
{
    public static class NQH_Utility
    {
        public static bool IsPlayerControlledNQH(this Thing thing)
        {
            return thing is NQH_Pawn p && p.Faction.IsPlayer;
        }
    }
}
