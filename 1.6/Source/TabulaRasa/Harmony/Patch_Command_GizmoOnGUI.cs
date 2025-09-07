using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.Planet;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Command), "GizmoOnGUI")]
    public static class Patch_Command_GizmoOnGUI
    {
		[HarmonyPrefix]
		public static bool Prefix(Command __instance, Vector2 topLeft, float maxWidth, GizmoRenderParms parms, GizmoResult __result)
		{
            if (TabulaRasaMod.settings.enableShrunkOrders)
            {
                // Awful way to handle this vanilla code fix but fuck it, Ludeon want to do weirdly stubborn code then so will I.
                // Just allows the max width to actually affect the scale of the gizmo, since otherwise the height is hard coded.
                __result = __instance.GizmoOnGUIInt(new Rect(topLeft.x, topLeft.y, __instance.GetWidth(maxWidth), __instance.GetWidth(maxWidth)), parms);
                return false;
            }
            return true;
        }
    }
}
