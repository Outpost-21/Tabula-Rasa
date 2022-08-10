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
    [HarmonyPatch(typeof(InspirationHandler), "InspirationHandlerTick")]
    public static class Patch_InspirationHandler_InspirationHandlerTick
    {
        [HarmonyPrefix]
        public static bool Prefix(ref InspirationHandler __instance)
        {
            if (__instance.pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
            {
                return false;
            }
            return true;
        }

    }
}
