using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.TurretsPlus;
using RimWorld.Planet;
using System.Reflection;
using System.Reflection.Emit;
using O21Toolbox.BiomeExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(WorldGenStep_Terrain), "BiomeFrom")]
    public static class Transpiler_WorldGenStep_Terrain_BiomeFrom
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> WorldGenStep_Terrain_BiomeFrom_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo workerCall = AccessTools.Method(typeof(BiomeWorker), "GetScore");
            MethodInfo getWorker = AccessTools.Method(typeof(BiomeDef), "get_Worker");
            BiomeDef def = null;
            foreach (CodeInstruction codeInstruction in instructions)
            {
                yield return codeInstruction;
                if(def == null && codeInstruction.opcode == OpCodes.Ldobj && ((BiomeDef)codeInstruction.operand)?.workerClass == typeof(BiomeWorker_Universal))
                {
                    def = (BiomeDef)codeInstruction.operand;
                }
                if(codeInstruction.opcode == OpCodes.Callvirt && codeInstruction.OperandIs(getWorker))
                {

                }
                if (codeInstruction.opcode == OpCodes.Callvirt && codeInstruction.OperandIs(workerCall))
                {
                }
            }
        }
    }
}
