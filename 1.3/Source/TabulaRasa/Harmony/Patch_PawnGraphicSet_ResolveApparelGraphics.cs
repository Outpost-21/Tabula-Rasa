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
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class Patch_PawnGraphicSet_ResolveApparelGraphics
    {
        [HarmonyPrefix]
        public static void Prefix(PawnGraphicSet __instance)
        {
            Patch_ApparelGraphicRecordGetter_TryGetGraphicApparel.nextRaceDefName = __instance.pawn?.def == ThingDefOf.Human ? null : __instance.pawn?.def.defName;
        }
    }
}
