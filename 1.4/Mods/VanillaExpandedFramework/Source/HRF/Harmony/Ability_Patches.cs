using HarmonyLib;
using MVCF.Utilities;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using VFECore.Abilities;

namespace HRF
{
    [HotSwappable]
    [HarmonyPatch(typeof(Ability), "IsEnabledForPawn")]
    public static class Patch_IsEnabledForPawn
    {
        private static void Postfix(ref bool __result, Ability __instance, ref string reason)
        {
            var extension = __instance.def.GetModExtension<AbilityResourceProps>();
            if (extension != null && __result)
            {
                var isUsable = Utils.IsUsableForProps(__instance.pawn, extension, out var reason2);
                if (!isUsable)
                {
                    reason = reason2;
                    __result = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Ability), "Cast", new Type[] { typeof(GlobalTargetInfo[]) })]
    public static class Patch_Cast
    {
        private static void Postfix(Ability __instance, GlobalTargetInfo[] targets)
        {
            var extension = __instance.def.GetModExtension<AbilityResourceProps>();
            if (extension != null)
            {
                var targetThings = targets.Where(x => x.HasThing).Select(x => x.Thing).ToList();
                Utils.ApplyResourceSettings(targetThings, __instance.pawn, extension);
            }
        }
    }

    [HarmonyPatch(typeof(Ability), "GetDescriptionForPawn")]
    public static class Patch_GetDescriptionForPawn
    {
        private static void Postfix(Ability __instance, ref string __result)
        {
            var extension = __instance.def.GetModExtension<AbilityResourceProps>();
            if (extension != null)
            {
                StringBuilder sb = new StringBuilder(__result);
                sb.AppendLine(Utils.GetPropsDescriptions(__instance.pawn, extension));
                __result = sb.ToString().TrimEndNewlines();
            }
        }
    }
}