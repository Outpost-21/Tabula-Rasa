using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Drones;
using O21Toolbox.PawnKindExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_PawnGeneration
    {
    }

	[HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")]
	public static class Patch_PawnGen_FinalLevelOfSkill
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, Pawn pawn, SkillDef sk)
        {
            DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();

            if(modExt != null)
            {
                if (!modExt.skillSettings.NullOrEmpty())
                {
                    if(!modExt.skillSettings.Where(sr => sr.skill == sk).EnumerableNullOrEmpty())
                    {
                        __result = modExt.skillSettings.Find(sr => sr.skill == sk).level;
                    }
                    else if (modExt.flattenSkills)
                    {
                        __result = 0;
                    }
                }
            }
        }
	}

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class Patch_PawnGen_TryGenerateNewPawnInternal
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo raceDef = AccessTools.Field(typeof(PawnKindDef), nameof(PawnKindDef.race));

            var codes = new List<CodeInstruction>(instructions);
            foreach(CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(raceDef))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(Patch_PawnGen_TryGenerateNewPawnInternal), nameof(GetAltRaceDef));
                }
                yield return instruction;
            }
        }

        public static ThingDef GetAltRaceDef(PawnKindDef pawnkind)
        {
            if (pawnkind.HasModExtension<DefModExt_ExtendedPawnKind>())
            {
                DefModExt_ExtendedPawnKind modExt = pawnkind.GetModExtension<DefModExt_ExtendedPawnKind>();
                ThingDefEntry resultEntry;
                if (!modExt.altRaces.NullOrEmpty())
                {
                    Func<ThingDefEntry, float> selector = (ThingDefEntry x) => x.weight;
                    resultEntry = modExt.altRaces.RandomElementByWeight(selector);

                    return resultEntry.races.RandomElement();
                }
            }

            return pawnkind.race;
        }
    }
}
