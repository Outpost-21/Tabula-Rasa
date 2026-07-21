using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class Gizmo_ShuttleTurret : Gizmo
    {
        public ShuttleTurret turret;

        public override float GetWidth(float maxWidth)
        {
            return 136f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect outRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect inRect = outRect.ContractedBy(6f);
            Widgets.DrawWindowBackground(outRect);
            // TODO: Draw Turret Icon to left.
            // TODO: Draw Turret proxy gizmos to the right, at shrunken scale for two rows. (Set target, hold fire, etc)
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
