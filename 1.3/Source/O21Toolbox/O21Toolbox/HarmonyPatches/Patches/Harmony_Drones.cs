using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using UnityEngine;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

using HarmonyLib;

using O21Toolbox.Drones;

namespace O21Toolbox.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public class Harmony_Drones
    {
        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {
            //O21ToolboxHarmony.Patch(typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(NQHFixerAncient)));
            //O21ToolboxHarmony.Patch( typeof(CompWakeUpDormant).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First( mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(NQHFixer)));
        }
        public static void DroneFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (typeof(Drone_Pawn).IsAssignableFrom(kind.race.thingClass)) __result = false;
        }

        public static void DroneFixer(ref bool __result, PawnKindDef def)
        {
            if (typeof(Drone_Pawn).IsAssignableFrom(def.race.thingClass)) __result = false;
        }

        //[HarmonyPatch(typeof(TransferableUtility), "CanStack")]
        //public static class TransferableUtility_CanStack_Patch
        //{
        //    static bool Prefix(Thing thing, ref bool __result)
        //    {
        //        if (thing.def.category == ThingCategory.Pawn)
        //        {
        //            Pawn pawn = (Pawn)thing;
        //            if (pawn is Drone_Pawn)
        //            {
        //                __result = false;
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //}

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("ShouldSendNotificationAbout")]
        public static class ShouldSendNotificationPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn p)
            {
                return !(p is Drone_Pawn);
            }
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch("IsColonistPlayerControlled", MethodType.Getter)]
        public static class IsColonistPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn __instance, ref bool __result)
            {
                if (__instance is Drone_Pawn)
                {
                    __result = __instance.Spawned && (__instance.Faction != null && __instance.Faction.IsPlayer) && __instance.MentalStateDef == null && __instance.HostFaction == null;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn), "GetDisabledWorkTypes")]
        public static class GetDisabledWorkTypesPrefix
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Pawn __instance, ref List<WorkTypeDef> __result, bool permanentOnly)
            {
                if (__instance.def.HasModExtension<DefModExt_Drone>())
                {
                    DefModExt_Drone modExt = __instance.def.GetModExtension<DefModExt_Drone>();
                    __result = modExt.disabledWorkTypes;
                    return false;
                }
                return true;
            }
        }

        //[HarmonyPatch(typeof(TransferableUtility), nameof(TransferableUtility.CanStack))]
        //public static class CanStackDroneTranspiler
        //{
        //    [HarmonyTranspiler]
        //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        //    {
        //        List<CodeInstruction> instructionList = instructions.ToList();

        //        for (int i = 0; i < instructionList.Count; i++)
        //        {
        //            CodeInstruction instruction = instructionList[i];
        //            if (instruction.opcode == OpCodes.Stloc_0)
        //            {
        //                Label label = ilg.DefineLabel();

        //                i++;
        //                yield return instruction;
        //                yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);
        //                yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(Drone_Utility), nameof(Drone_Utility.IsDrone), new Type[] { typeof(Pawn) }));
        //                yield return new CodeInstruction(opcode: OpCodes.Brfalse, label);

        //                yield return new CodeInstruction(opcode: OpCodes.Ldc_I4_0);
        //                yield return new CodeInstruction(opcode: OpCodes.Ret);
        //                instruction = instructionList[i];
        //                instruction.labels.Add(label);
        //            }
        //            yield return instruction;
        //        }
        //    }
        //}
    }
}
