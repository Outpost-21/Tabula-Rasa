using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Shield
{
    [StaticConstructorOnStartup]
    public class Gizmo_ShieldStressStatus : Gizmo
    {
		public Building_Shield shieldGen;

		private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

		private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

		public Gizmo_ShieldStressStatus()
        {
            this.order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
		{
			Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
			Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
			{
				Rect rect = overRect.AtZero().ContractedBy(6f);
				Rect rect2 = rect;
				rect2.height = overRect.height / 2f;
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect2, this.shieldGen.LabelCap);
				Rect rect3 = rect;
				rect3.yMin = overRect.height / 2f;
				float num = this.shieldGen.active ? this.shieldGen.CurShieldStress : 1f;
				float fillPercent = num / this.shieldGen.MaxShieldStress;
				Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect3, (num * 100f).ToString("F0") + " / " + (this.shieldGen.MaxShieldStress * 100f).ToString("F0"));
				Text.Anchor = TextAnchor.UpperLeft;
			}, true, false, 1f);
			return new GizmoResult(GizmoState.Clear);
		}
	}
}
