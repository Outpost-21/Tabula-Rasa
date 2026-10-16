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
    [HarmonyPatch(typeof(PawnGroupMaker), "CanGenerateFrom")]
    public class Patch_PawnGroupMaker_CanGenerateFrom
    {
        [HarmonyPostfix]
        public static void Postfix(PawnGroupMaker __instance, PawnGroupMakerParms parms, ref bool __result)
        {
            PawnGroupMaker_FactionPoints pawnGroupMaker_FactionPoints = __instance as PawnGroupMaker_FactionPoints;
            if (pawnGroupMaker_FactionPoints != null)
            {
                __result &= pawnGroupMaker_FactionPoints.CanGenerate(parms);
                return;
            }

            PawnGroupMaker_Temperature pawnGroupMaker_Temperature = __instance as PawnGroupMaker_Temperature;
            if (pawnGroupMaker_Temperature != null)
            {
                __result &= pawnGroupMaker_Temperature.CanGenerate(parms);
                return;
            }
        }
    }
}
