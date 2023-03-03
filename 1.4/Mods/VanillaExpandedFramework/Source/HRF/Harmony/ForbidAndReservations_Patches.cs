using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{

    [HarmonyPatch(typeof(ReservationManager), "CanReserve")]
    public static class Patch_CanReserve
    {
        private static void Postfix(ref bool __result, Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
        {
            if (__result)
            {
                var thing = target.Thing;
                if (thing != null && !claimant.CanUseIt(thing, out var failReason))
                {
                    JobFailReason.Is(failReason);
                    __result = false;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(ForbidUtility), "IsForbidden", new Type[] { typeof(Thing), typeof(Pawn) })]
    public static class Patch_IsForbidden
    {
        private static void Postfix(ref bool __result, Thing t, Pawn pawn)
        {
            if (!__result)
            {
                if (!pawn.CanUseIt(t, out var failReason))
                {
                    JobFailReason.Is(failReason);
                    __result = true;
                }
            }
        }
    }
}
