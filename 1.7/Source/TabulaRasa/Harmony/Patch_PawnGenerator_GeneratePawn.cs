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

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(PawnGenerationRequest request, Pawn __result)
        {
            Pawn pawn = __result;
            DefModExt_PawnKindExtended modExt = pawn.kindDef.GetModExtension<DefModExt_PawnKindExtended>();
            if (modExt != null && modExt.clearApparel)
            {
                for (int i = 0; i < pawn.apparel.WornApparel.Count(); i++)
                {
                    pawn.apparel.Remove(pawn.apparel.WornApparel[i]);
                }
            }
        }
    }
}
