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
    [HarmonyPatch(typeof(Building_Door), "DoorPowerOn", MethodType.Getter)]
    public static class Patch_Building_Door_DoorPowerOn
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Building_Door __instance)
        {
            if (__instance.def.HasModExtension<DefModExt_SelfPoweredDoor>())
            {
                __result = true;
            }
        }
    }
}
