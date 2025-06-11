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
    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")]
    public static class Patch_PawnGenerator_FinalLevelOfSkill
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, Pawn pawn, SkillDef sk)
        {
            DefModExt_PawnKindExtended modExt = pawn.kindDef.GetModExtension<DefModExt_PawnKindExtended>();

            if (modExt != null)
            {
                if (!modExt.skillSettings.NullOrEmpty())
                {
                    if (!modExt.skillSettings.Where(sr => sr.skill == sk).EnumerableNullOrEmpty())
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
}
