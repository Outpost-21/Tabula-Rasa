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
    [HarmonyPatch(typeof(StunHandler), "AffectedByEMP", MethodType.Getter)]
    public class Patch_StunHandler_AffectedByEMP
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ref StunHandler __instance, Thing ___parent)
        {
            if (___parent is Pawn pawn && pawn != null)
            {
                DefModExt_ArtificialPawn modExt = pawn.def.GetModExtension<DefModExt_ArtificialPawn>();
                if (modExt != null)
                {
                    if (pawn.def.GetModExtension<DefModExt_ArtificialPawn>().affectedByEMP)
                    {
                        if (pawn.apparel != null && pawn.apparel.WornApparel.Any(a => a.def.HasModExtension<DefModExt_EMPShielding>()))
                        {
                            __result = false;
                            return;
                        }
                        if ((pawn.health?.hediffSet?.hediffs?.Any(h => h.def.HasModExtension<DefModExt_EMPShielding>()) ?? false))
                        {
                            __result = false;
                            return;
                        }
                        __result = true;
                    }
                }
            }
        }
    }
}
