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
    [HarmonyPatch(typeof(HealthAIUtility), "ShouldEverReceiveMedicalCareFromPlayer")]
    public class Patch_HealthAIUtility_ShouldEverReceiveMedicalCareFromPlayer
    {
        [HarmonyPostfix]
        public static bool Prefix(Pawn pawn, bool __result)
        {
            if (pawn != null)
            {
                DefModExt_ArtificialPawn modExt = pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
                if (modExt != null && !modExt.canBeRepaired)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
