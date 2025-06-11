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
    [HarmonyPatch(typeof(PawnRenderTree), "ProcessApparel")]
    public static class Patch_PawnRenderTree_ProcessApparel
    {
        [HarmonyPrefix]
        public static void Prefix(PawnRenderTree __instance)
        {
            Patch_ApparelGraphicRecordGetter_TryGetGraphicApparel.curHeadTypeDef = __instance.pawn?.story?.headType?.defName ?? null;
        }
    }
}
