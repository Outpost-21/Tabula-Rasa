using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public abstract class BaseVerb : Verb
    {
        public override bool MultiSelect => true;
        public VerbProps Props => verbProps as VerbProps;

        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && verbProps.targetParams.CanTarget(target.ToTargetInfo(caster.Map)))
            {
                base.OnGUI(target);
                return;
            }

            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid && CanHitTarget(target))
            {
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
                DrawHighlightFieldRadiusAroundTarget(target);
            }

            if (verbProps.requireLineOfSight)
            {
                GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white, c =>
                    GenSight.LineOfSight(caster.Position, c, caster.Map));
                return;
            }

            GenDraw.DrawRadiusRing(caster.Position, EffectiveRange);
        }
    }
}