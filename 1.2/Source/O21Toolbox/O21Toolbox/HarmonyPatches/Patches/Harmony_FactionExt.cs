using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.FactionExt;

namespace O21Toolbox.HarmonyPatches
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
            if(pawnGroupMaker_Temperature != null)
            {
                __result &= pawnGroupMaker_Temperature.CanGenerate(parms);
                return;
            }
        }
    }
}
