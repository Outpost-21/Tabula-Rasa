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
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public class Patch_SkillRecord_Learn
    {
        [HarmonyPrefix]
        public static bool Prefix(float xp, bool direct, SkillRecord __instance)
        {
            Pawn pawn = __instance?.pawn;
            DefModExt_ArtificialPawn modExt = pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
            if (modExt != null)
            {
                if (modExt.noSkillGain)
                {
                    return false;
                }
                else if (xp > 0f)
                {
                    xp *= modExt.skillGainMulti;
                }
                else if(xp < 0f)
                {
                    xp *= modExt.skillLossMulti;
                }
            }
            return true;
        }
    }
}
