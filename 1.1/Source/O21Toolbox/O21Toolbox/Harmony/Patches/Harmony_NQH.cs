using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

using Harmony;

using O21Toolbox.NotQuiteHumanoid;

namespace O21Toolbox.Harmony.Patches
{
    public class Harmony_NQH
    {
        public static void Harmony_Patch(HarmonyInstance O21ToolboxHarmony, Type patchType)
        {
            O21ToolboxHarmony.Patch(
                typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(mi =>
                        mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                        mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)),
                null, new HarmonyMethod(typeof(HarmonyPatches),
                    nameof(NQHFixerAncient)));
            O21ToolboxHarmony.Patch(
                typeof(CompWakeUpDormant).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First(
                    mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                          mi.GetParameters().Count() == 1 &&
                          mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(
                    typeof(HarmonyPatches),
                    nameof(NQHFixer)));
        }

        public static void NQHFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (typeof(NQH_Pawn).IsAssignableFrom(kind.race.thingClass)) __result = false;
        }

        public static void NQHFixer(ref bool __result, PawnKindDef def)
        {
            if (typeof(NQH_Pawn).IsAssignableFrom(def.race.thingClass)) __result = false;
        }

        [HarmonyPatch(typeof(TransferableUtility), "CanStack")]
        public static class TransferableUtility_CanStack_Patch
        {
            static bool Prefix(Thing thing, ref bool __result)
            {
                if (thing.def.category == ThingCategory.Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    if (pawn is NQH_Pawn)
                    {
                        __result = false;
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("ShouldSendNotificationAbout")]
        public static class ShouldSendNotificationPatch
        {
            public static bool Prefix(Pawn p)
            {
                return !(p is NQH_Pawn);
            }
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch("IsColonistPlayerControlled", MethodType.Getter)]
        public static class IsColonistPatch
        {
            public static void Postfix(Pawn __instance, ref bool __result)
            {
                if (__instance is NQH_Pawn)
                {
                    __result = __instance.Spawned && (__instance.Faction != null && __instance.Faction.IsPlayer) && __instance.MentalStateDef == null && __instance.HostFaction == null;
                }
            }
        }
    }
}
