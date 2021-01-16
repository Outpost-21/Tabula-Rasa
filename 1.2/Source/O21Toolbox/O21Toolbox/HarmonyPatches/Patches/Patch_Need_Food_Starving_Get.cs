using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(Need_Food), "Starving", MethodType.Getter)]
    public class Patch_Need_Food_Starving_Get
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn ___pawn)
        {
            if (___pawn != null && ___pawn.def.race.FleshType.IsArtificialPawn())
            {
                __result = false;
            }
        }
    }
}
