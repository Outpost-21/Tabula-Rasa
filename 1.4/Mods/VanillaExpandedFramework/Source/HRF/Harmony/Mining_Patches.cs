using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{

    [HarmonyPatch(typeof(WorkGiver_Miner), "JobOnThing")]
    public static class Patch_WorkGiver_Miner_JobOnThing
    {
        public static bool Prefix(ref Job __result, WorkGiver_Miner __instance, Pawn pawn, Thing t, bool forced = false)
        {
            var extension = t.def.GetModExtension<Extension_ThingInUse>();
            if (extension != null)
            {
                foreach (var useProps in extension.useProperties)
                {
                    if (!pawn.CanUseIt(t.Label, useProps, useProps.resourceOnStrike, useProps.cannotMineMessageKey, out var failMessage))
                    {
                        JobFailReason.Is(failMessage);
                        __result = null;
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch]
    public static class JobDriver_Mine_Patch
    {
        [HarmonyTargetMethod]
        public static MethodBase GetMethod()
        {
            return typeof(JobDriver_Mine).GetNestedTypes(AccessTools.all).First().GetMethods(AccessTools.all).Where(x => x.Name.Contains("<MakeNewToils>")).First();
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, MethodBase method)
        {
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (i > 1 && codes[i].opcode == OpCodes.Bgt && codes[i - 1].opcode == OpCodes.Ldc_I4_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, method.DeclaringType.GetField("<>4__this"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Mine_Patch), "RegisterHit"));
                }
            }
        }

        public static void RegisterHit(JobDriver_Mine jobDriver_Mine)
        {
            var mineableDef = jobDriver_Mine.job.GetTarget(TargetIndex.A).Thing.def;
            var extension = mineableDef.GetModExtension<Extension_ThingInUse>();
            if (extension != null)
            {
                foreach (var useProps in extension.useProperties)
                {
                    if (useProps.resourceOnStrike != 0)
                    {
                        Utils.AdjustResourceAmount(jobDriver_Mine.pawn, useProps.hediff, useProps.resourceOnStrike, useProps.addHediffIfMissing, null, null);
                    }
                }
            }
        }
    }
}