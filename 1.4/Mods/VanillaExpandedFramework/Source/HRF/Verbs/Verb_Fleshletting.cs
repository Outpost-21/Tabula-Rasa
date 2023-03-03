using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_Fleshletting : BaseVerb
    {
        public override bool TryCastShot()
        {
            var pawn = currentTarget.Pawn;
            if (pawn == null) return false;
            if (pawn.health.hediffSet.HasHediff(Props.applyHediff)) return false;
            pawn.health.AddHediff(Props.applyHediff);
            var meatNum = pawn.GetStatValue(StatDefOf.MeatAmount) * Props.meatYield;
            if (meatNum >= 0)
            {
                var thing = ThingMaker.MakeThing(pawn.def.race.meatDef);
                thing.stackCount = (int) meatNum;
                GenPlace.TryPlaceThing(thing, currentTarget.Cell, caster.Map, ThingPlaceMode.Near);
            }

            var leatherNum = pawn.GetStatValue(StatDefOf.LeatherAmount) * Props.leatherYield;
            if (leatherNum >= 0)
            {
                var thing = ThingMaker.MakeThing(pawn.def.race.leatherDef);
                thing.stackCount = (int) leatherNum;
                GenPlace.TryPlaceThing(thing, currentTarget.Cell, caster.Map, ThingPlaceMode.Near);
            }

            return true;
        }

        public override bool CanHitTarget(LocalTargetInfo targ)
        {
            var pawn = targ.Pawn;
            if (pawn == null) return false;
            if (pawn.health.hediffSet.HasHediff(Props.applyHediff)) return false;
            return base.ValidateTarget(targ);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            var pawn = target.Pawn;
            if (pawn == null) return false;
            if (pawn.health.hediffSet.HasHediff(Props.applyHediff)) return false;
            return base.ValidateTarget(target, showMessages);
        }
    }
}