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
            if (___parent is Pawn)
            {
                DefModExt_ArtificialPawn modExt = ___parent.def.GetModExtension<DefModExt_ArtificialPawn>();
                if(modExt != null)
                {
                    __result = modExt.affectedByEMP;
                }
            }
        }
    }
}
