using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;
using O21Toolbox.RoyaltyExt;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), "AssignHeirIfNone")]
    public class Patch_RoyaltyTracker_AssignHeirIfNone
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_RoyaltyTracker __instance, RoyalTitleDef t, Faction faction)
        {
            if (faction.def.HasModExtension<DefModExt_NoInherit>())
            {
                return false;
            }

            return true;
        }
    }
}
