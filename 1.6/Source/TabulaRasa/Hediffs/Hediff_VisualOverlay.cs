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
	[StaticConstructorOnStartup]
    public abstract class Hediff_VisualOverlay : HediffWithComps
	{
		public MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();

		private Material material;

		public Material OverlayMat
		{
			get
			{
				if (material == null)
				{
					material = MaterialPool.MatFrom(OverlayPath, OverlayShader);
				}
				return material;
			}
		}

		public virtual float OverlaySize => 1f;

		public virtual string OverlayPath { get; }

		public virtual Shader OverlayShader => ShaderDatabase.MoteGlow;

		public virtual void Draw()
        {

		}

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			pawn.MapHeld?.GetComponent<MapComp_VisualOverlays>().hediffOverlays.Add(this);
		}
	}
}
