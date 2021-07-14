using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.Hibernation;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(Thing), "Suspended", MethodType.Getter)]
    public static class Patch_Thing_Suspended
    {
        [HarmonyPostfix]
        public static void Postfix(ref Thing __instance, ref bool __result)
        {
            if (__instance != null && __instance.def.HasModExtension<DefModExt_Hibernation>())
            {
                DefModExt_Hibernation modExt = __instance.def.GetModExtension<DefModExt_Hibernation>();
                Pawn pawn = __instance as Pawn;
                if (pawn != null)
                {
                    if(pawn.CurJobDef == modExt.hibernationJob && pawn.PositionHeld == pawn.CurJob.targetA.Thing.Position)
                    {
                        __result = true;
                    }
                }
            }
        }
    }
}
