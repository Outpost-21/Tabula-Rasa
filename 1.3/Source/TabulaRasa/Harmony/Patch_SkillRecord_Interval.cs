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
    [HarmonyPatch(typeof(SkillRecord), "Interval")]
    public class Patch_SkillRecord_Interval
    {
        [HarmonyPrefix]
        public static bool Prefix(SkillRecord __instance)
        {
            Pawn pawn = __instance.pawn;
            DefModExt_ArtificialPawn modExt = pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
            return modExt == null || !modExt.noSkillLoss;
        }
    }
}
