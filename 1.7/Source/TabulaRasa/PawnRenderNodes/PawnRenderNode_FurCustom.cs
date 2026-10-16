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
    public class PawnRenderNode_FurCustom : PawnRenderNode
	{
		public override Shader DefaultShader => ShaderDatabase.CutoutSkinOverlay;

		public PawnRenderNode_FurCustom(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			if (!ModLister.CheckBiotech("Fur"))
			{
				return null;
			}
			if (pawn.story?.furDef == null)
			{
				return null;
            }
			Color furColor;
			switch (props.colorType)
            {
				case PawnRenderNodeProperties.AttachmentColorType.Custom:
					furColor = props.color ?? pawn.story.hairColor;
					break;
				case PawnRenderNodeProperties.AttachmentColorType.Skin:
					furColor = pawn.story.SkinColor;
					break;
				default:
					furColor = pawn.story.HairColor;
					break;
            }
            return GraphicDatabase.Get<Graphic_Multi>(pawn.story?.furDef.GetFurBodyGraphicPath(pawn), ShaderFor(pawn), Vector2.one, furColor);
		}

        public override Color ColorFor(Pawn pawn)
        {
            return base.ColorFor(pawn);
        }
    }
}
