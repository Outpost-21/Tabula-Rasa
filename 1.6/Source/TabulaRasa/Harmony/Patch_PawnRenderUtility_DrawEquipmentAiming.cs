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
    [HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming")]
    public static class Patch_PawnRenderUtility_DrawEquipmentAiming
    {
        [HarmonyPrefix]
        public static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if(eq != null && eq.def.HasModExtension<DefModExt_InvisibleWeapon>())
            {
                return false;
            }
            return true;
        }
    }
}
