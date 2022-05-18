using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;

namespace O21Toolbox
{
    [HarmonyPatch(typeof(Building_Door), "DoorPowerOn", MethodType.Getter)]
    public class Patch_Building_Door_DoorPowerOn
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
