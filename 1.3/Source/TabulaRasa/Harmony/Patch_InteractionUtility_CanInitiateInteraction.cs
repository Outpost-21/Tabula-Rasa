using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(InteractionUtility), "CanInitiateInteraction")]
    public static class Patch_InteractionUtility_CanInitiateInteraction
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref Pawn pawn)
        {
            DefModExt_ArtificialPawn modExtPawn = pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
            if (modExtPawn != null && !modExtPawn.canSocialize)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
