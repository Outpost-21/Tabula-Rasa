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
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "CanInteractNowWith")]
    public static class Patch_Pawn_InteractionsTracker_CanInteractNowWith
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_InteractionsTracker __instance, ref bool __result, ref Pawn recipient)
        {
            DefModExt_ArtificialPawn modExtPawn = __instance.pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
            DefModExt_ArtificialPawn modExtRecipient = recipient.def.GetModExtension<DefModExt_ArtificialPawn>();
            if ((modExtPawn != null && !modExtPawn.canSocialize) || (modExtRecipient != null && !modExtRecipient.canSocialize))
            {
                __result = false;
                return false;
            }
            return true;
        }

    }
}
