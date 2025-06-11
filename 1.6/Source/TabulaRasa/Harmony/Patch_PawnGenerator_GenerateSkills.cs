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

    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    public static class Patch_PawnGen_GenerateSkills
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            DefModExt_PawnKindExtended modExt = pawn.kindDef.GetModExtension<DefModExt_PawnKindExtended>();

            if (modExt != null)
            {
                if (modExt.clearPassions)
                {
                    for (int i = 0; i < pawn.skills.skills.Count(); i++)
                    {
                        pawn.skills.skills[i].passion = Passion.None;
                    }
                }
            }
        }
    }
}
