using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class Comp_IdeoIconOverlay : ThingComp
    {
        public CompProperties_IdeoIconOverlay Props => (CompProperties_IdeoIconOverlay)props;

        private Texture2D iconTexture;

        private Color ideoColor = Color.white;

        private Material Graphic
        {
            get
            {
                if(iconTexture == null)
                {
                    iconTexture = Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Icon;
                }
                return MaterialPool.MatFrom(new MaterialRequest(iconTexture));
            }
        }

        private bool ShouldDraw
        {
            get
            {
                if (parent.Rotation == Rot4.South)
                {
                    return Props.showSouth;
                }
                if (parent.Rotation == Rot4.North)
                {
                    return Props.showNorth;
                }
                if (parent.Rotation == Rot4.East)
                {
                    return Props.showEast;
                }
                if (parent.Rotation == Rot4.West)
                {
                    return Props.showWest;
                }
                return false;
            }
        }

        private Vector3 CurrentOffset
        {
            get
            {
                if(parent.Rotation == Rot4.South)
                {
                    return Props.offsetSouth;
                }
                if (parent.Rotation == Rot4.North)
                {
                    return Props.offsetNorth;
                }
                if (parent.Rotation == Rot4.East)
                {
                    return Props.offsetEast;
                }
                if (parent.Rotation == Rot4.West)
                {
                    return Props.offsetWest;
                }
                return new Vector3(0, 0, 0);
            }
        }

        private Color GetIdeoColor => Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.Color;

        public override void CompTickRare()
        {
            base.CompTickRare();

            iconTexture = null;
            ideoColor = GetIdeoColor;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if(ModsConfig.IdeologyActive && ShouldDraw)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                if (parent.Stuff != null && parent.Stuff.stuffProps.color != null)
                {
                    propertyBlock.SetColor(ShaderPropertyIDs.Color, ideoColor);
                }
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(parent.DrawPos + CurrentOffset, Quaternion.AngleAxis(0, Vector3.up), new Vector3(Props.drawSize.x, 1, Props.drawSize.y));
                Graphics.DrawMesh(MeshPool.plane10, matrix, Graphic, 0, null, 0, propertyBlock);
            }
        }
    }
}
