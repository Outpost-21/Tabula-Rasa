using System.Linq;
using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_Multishot : Verb_Shoot
    {
        public VerbProps Props => verbProps as VerbProps;

        public override bool TryCastShot()
        {
            var oldTarget = currentTarget;
            if (base.TryCastShot())
            {
                foreach (var pawn in GenRadial.RadialDistinctThingsAround(oldTarget.Cell, caster.Map,
                        Props.multishotRadius, false).Except(oldTarget.Thing).OfType<Pawn>()
                    .Where(p => Props.multishotTargetFriendly || p.HostileTo(caster)).Take(Props.multishotShots))
                {
                    currentTarget = pawn;
                    base.TryCastShot();
                }

                currentTarget = oldTarget;

                return true;
            }

            return false;
        }
    }
}