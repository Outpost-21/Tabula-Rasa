using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace TabulaRasa
{

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class Patch_PawnGen_TryGenerateNewPawnInternal
    {
        [HarmonyPrefix]
        static void Prefix(ref PawnGenerationRequest request)
        {
            PawnKindDef pawnKindDef = request.KindDef;
            if (Faction.OfPlayerSilentFail != null && pawnKindDef == PawnKindDefOf.Villager)
            {
                Faction faction = request.Faction;
                if (faction != null && faction.IsPlayer)
                {
                    ThingDef race = pawnKindDef.race;
                    Faction ofPlayer = Faction.OfPlayer;
                    if (race != ((ofPlayer != null) ? ofPlayer.def.basicMemberKind.race : null))
                    {
                        Faction ofPlayer2 = Faction.OfPlayer;
                        pawnKindDef = ((ofPlayer2 != null) ? ofPlayer2.def.basicMemberKind : null);
                    }
                }
            }
            request.KindDef = pawnKindDef;
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo raceDef = AccessTools.Field(typeof(PawnKindDef), nameof(PawnKindDef.race));

            var codes = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructions)
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
            ThingDef result = pawnkind.race;

            DefModExt_PawnKindRaces modExt = null;
            if (pawnkind.HasModExtension<DefModExt_PawnKindRaces>())
            {
                modExt = pawnkind.GetModExtension<DefModExt_PawnKindRaces>();
            }

            if (modExt != null)
            {
                if (!modExt.altRaces.NullOrEmpty())
                {
                    modExt.altRaces.Add(new WeightedRaceChoice(pawnkind.race, 100));
                    Func<WeightedRaceChoice, float> selector = (WeightedRaceChoice x) => x.weight;
                    return modExt.altRaces.RandomElementByWeight(selector).race;
                }
            }
            return result;
        }
    }
}
