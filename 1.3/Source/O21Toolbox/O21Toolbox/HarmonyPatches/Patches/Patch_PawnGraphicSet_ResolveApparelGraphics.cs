using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace O21Toolbox
{
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class Patch_PawnGraphicSet_ResolveApparelGraphics
    {
        [HarmonyPrefix]
        static void Prefix(PawnGraphicSet __instance)
        {
            Patch_ApparelGraphicRecordGetter_TryGetGraphicApparel.NextRaceDefName = __instance.pawn?.def == ThingDefOf.Human ? null : __instance.pawn?.def.defName;
        }
    }
}
