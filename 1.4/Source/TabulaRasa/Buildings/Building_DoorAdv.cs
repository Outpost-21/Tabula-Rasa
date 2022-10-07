using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Building_DoorAdv : Building_Door
    {
        public Graphic cachedASyncDoor;

        public Graphic cachedOverlay;

        public DefModExt_DoorAdv modExt;

        public override void PostMake()
        {
            base.PostMake();
            modExt = def.GetModExtension<DefModExt_DoorAdv>();
            if (modExt != null)
            {
                cachedASyncDoor = modExt.asyncDoorGraphic?.GraphicColoredFor(this);
                cachedOverlay = modExt.doorOverlayGraphic?.GraphicColoredFor(this);
                Log.Message(modExt.isSingle.ToString());
            }
        }

        public new float OpenPct
        {
            get
            {
                return Mathf.Clamp01(ticksSinceOpen / TicksToOpenNow);
            }
        }

        public override void Draw()
        {
            Vector2 vector = Graphic.drawSize;
            if (!(modExt != null && modExt.isSingle))
            {
                float d = 0f + Graphic.drawSize.x / 2 * OpenPct;
                for (int i = 0; i < 2; i++)
                {
                    Graphic graphic;
                    Vector3 vector3 = default(Vector3);
                    Mesh mesh;
                    if (i == 0)
                    {
                        graphic = Graphic;
                        vector3 += new Vector3(-1f, 0f, 0f);
                        mesh = MeshPool.GridPlane(vector);
                    }
                    else
                    {
                        graphic = cachedASyncDoor != null ? cachedASyncDoor : Graphic;
                        vector3 += new Vector3(1f, 0f, 0f);
                        mesh = MeshPool.GridPlaneFlip(vector);
                    }
                    Rot4 rot = Rotation;
                    if (Rot4.South == Rotation)
                        rot = Rot4.North;
                    vector3 = rot.AsQuat * vector3;
                    Vector3 drawPos = DrawPos;
                    drawPos.y = AltitudeLayer.DoorMoveable.AltitudeFor();
                    drawPos += vector3 * d;
                    drawPos += graphic.DrawOffset(Rotation);
                    Graphics.DrawMesh(mesh, drawPos, rot.AsQuat, graphic.MatAt(Rotation), 0);
                    graphic.ShadowGraphic?.DrawWorker(drawPos, i == 0 ? Rotation : Rotation.Opposite, def, this, 0f);
                }
            }
            else
            {
                float d = 0f + Graphic.drawSize.x * OpenPct;
                Vector3 vector3 = default(Vector3);
                vector3 += new Vector3(-1f, 0f, 0f);
                Mesh mesh = MeshPool.GridPlane(vector);
                Rot4 rot = Rotation;
                if (Rot4.South == Rotation)
                    rot = Rot4.North;
                vector3 = rot.AsQuat * vector3;
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.DoorMoveable.AltitudeFor();
                drawPos += vector3 * d;
                drawPos += Graphic.DrawOffset(Rotation);
                Graphics.DrawMesh(mesh, drawPos, rot.AsQuat, Graphic.MatAt(Rotation), 0);
                Graphic.ShadowGraphic?.DrawWorker(drawPos, Rotation, def, this, 0f);
            }
            if(cachedOverlay != null)
            {
                float d = 0f + cachedOverlay.drawSize.x;
                Vector3 vector3 = default(Vector3);
                vector3 += new Vector3(-1f, 0f, 0f);
                Mesh mesh = MeshPool.GridPlane(vector);
                Rot4 rot = Rotation;
                if (Rot4.South == Rotation)
                    rot = Rot4.North;
                vector3 = rot.AsQuat * vector3;
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.DoorMoveable.AltitudeFor();
                drawPos += vector3 * d;
                drawPos += cachedOverlay.DrawOffset(Rotation);
                Graphics.DrawMesh(mesh, drawPos, rot.AsQuat, cachedOverlay.MatAt(Rotation), 0);
                cachedOverlay.ShadowGraphic?.DrawWorker(drawPos, Rotation, def, this, 0f);
            }
            Comps_PostDraw();
        }
    }
}
