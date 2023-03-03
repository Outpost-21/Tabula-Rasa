using UnityEngine;
using Verse;

namespace HRF
{
    [StaticConstructorOnStartup]
    public class Command_SwitchHediffStageResource : Command_Action
    {
        private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(Color.gray.r, Color.gray.g, Color.gray.b, 0.6f);

        private HediffResource hediffResource;
        public Command_SwitchHediffStageResource(HediffResource hediffResource)
        {
            this.hediffResource = hediffResource;
            order = 5f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            if (hediffResource.lastStageActivatedTick > 0)
            {
                var cooldownTicksRemaining = Find.TickManager.TicksGame - hediffResource.lastStageActivatedTick;
                if (cooldownTicksRemaining < hediffResource.curCooldownPeriod)
                {
                    float num = Mathf.InverseLerp(hediffResource.curCooldownPeriod, 0, cooldownTicksRemaining);
                    Widgets.FillableBar(rect, Mathf.Clamp01(num), cooldownBarTex, null, doBorder: false);
                }
            }
            if (hediffResource.lastStageSwitchTick > 0)
            {
                var cooldownTicksRemaining = Find.TickManager.TicksGame - hediffResource.lastStageSwitchTick;
                if (cooldownTicksRemaining < hediffResource.curChangeTime)
                {
                    float num = Mathf.Abs(1 - Mathf.InverseLerp(hediffResource.curChangeTime, 0, cooldownTicksRemaining));
                    FillableBarVertical(rect, Mathf.Clamp01(num), cooldownBarTex, null, doBorder: false);
                }
            }
            if (result.State == GizmoState.Interacted)
            {
                return result;
            }
            return new GizmoResult(result.State);
        }

        public static Rect FillableBarVertical(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
        {
            if (doBorder)
            {
                GUI.DrawTexture(rect, BaseContent.BlackTex);
                rect = rect.ContractedBy(3f);
            }
            if (bgTex != null)
            {
                GUI.DrawTexture(rect, bgTex);
            }
            Rect result = rect;
            rect.height *= fillPercent;
            GUI.DrawTexture(rect, fillTex);
            return result;
        }
    }
}
