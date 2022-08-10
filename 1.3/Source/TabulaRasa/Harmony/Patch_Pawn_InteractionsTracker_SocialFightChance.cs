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
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "SocialFightChance")]
    public static class Patch_Pawn_InteractionsTracker_SocialFightChance
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_InteractionsTracker __instance, ref float __result, ref InteractionDef interaction, ref Pawn initiator)
        {
            DefModExt_ArtificialPawn modExtPawn = __instance.pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
            DefModExt_ArtificialPawn modExtInitiator = initiator.def.GetModExtension<DefModExt_ArtificialPawn>();
            if ((modExtPawn != null && !modExtPawn.canSocialize) || (modExtInitiator != null && !modExtInitiator.canSocialize))
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }
}
