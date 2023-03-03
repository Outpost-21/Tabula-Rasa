using System.Linq;
using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_ConsumeCorpse : BaseVerb
    {
        private Corpse GetCorpse(LocalTargetInfo target)
        {
            return target.Cell.GetThingList(caster.Map).OfType<Corpse>().FirstOrDefault(c =>
                Props.requireRotStage == null || c.GetRotStage() == Props.requireRotStage);
        }

        public override bool TryCastShot()
        {
            var corpse = GetCorpse(currentTarget);
            if (corpse == null) return false;
            foreach (var option in Props.TargetResourceSettings)
                Utils.AdjustResourceAmount(CasterPawn,
                    option.hediff, option.resourcePerUse,
                    option.addHediffIfMissing, null, null);

            corpse.Destroy();

            return true;
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return base.ValidateTarget(target, showMessages) && GetCorpse(target) != null;
        }

        public override bool CanHitTarget(LocalTargetInfo targ)
        {
            return base.CanHitTarget(targ) && GetCorpse(targ) != null;
        }
    }
}