using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_PullTarget : BaseVerb
    {
        public override bool TryCastShot()
        {
            var pawn = CurrentTarget.Pawn;
            if (pawn == null) return false;
            var cell = caster.RandomAdjacentCell8Way();
            var flyer = PawnFlyer.MakeFlyer(ThingDef.Named("PulledPawn"), pawn, cell, null, null);
            if (flyer == null) return false;
            GenSpawn.Spawn(flyer, cell, caster.Map);
            return true;
        }
    }

    public class PulledPawn : PawnMover
    {
    }
}