using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace TabulaRasa
{
    public class Building_MultiMoverDoor : Building_SupportedDoor
    {
        public DefModExt_MultiMoverDoor modExt;

        public Graphic GraphicRight
        {
            get
            {
                if (graphicRightInt == null)
                {
                    if (modExt.rightMoverData == null)
                    {
                        return BaseContent.BadGraphic;
                    }
                    graphicRightInt = modExt.rightMoverData.GraphicColoredFor(this);
                }
                return graphicRightInt;
            }
        }

        public Graphic graphicRightInt;

        public override bool CanDrawMovers => false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            modExt = def.GetModExtension<DefModExt_MultiMoverDoor>();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            DoorPreDraw();
            float offsetDist = 0f + 0.45f * OpenPct;
            DrawMover(drawLoc, offsetDist, Graphic, AltitudeLayer.DoorMoveable.AltitudeFor(), Vector3.one, Graphic.ShadowGraphic, false);
            DrawMover(drawLoc, offsetDist, GraphicRight, AltitudeLayer.DoorMoveable.AltitudeFor(), Vector3.one, GraphicRight.ShadowGraphic, true);
            base.DrawAt(drawLoc, flip);
        }

        public void DrawMover(Vector3 drawPos, float offsetDist, Graphic graphic, float altitude, Vector3 drawScaleFactor, Graphic_Shadow shadowGraphic, bool flipped)
        {
            Mesh mesh;
            Vector3 vector;
            if (!flipped)
            {
                vector = new Vector3(0f, 0f, -def.size.x);
                mesh = MeshPool.plane10;
            }
            else
            {
                vector = new Vector3(0f, 0f, def.size.x);
                mesh = MeshPool.plane10Flip;
            }
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            vector = rotation.AsQuat * vector;
            Vector3 vector2 = drawPos;
            vector2.y = altitude;
            vector2 += vector * offsetDist;
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(vector2, base.Rotation.AsQuat, new Vector3((float)def.size.x * drawScaleFactor.x, drawScaleFactor.y, (float)def.size.z * drawScaleFactor.z)), graphic.MatAt(base.Rotation, this), 0);
            shadowGraphic?.DrawWorker(vector2, base.Rotation, def, this, 0f);
        }
    }
}
