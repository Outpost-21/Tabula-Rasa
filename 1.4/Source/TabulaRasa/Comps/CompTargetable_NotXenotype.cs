using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TabulaRasa
{
    public class CompTargetable_NotXenotype : CompTargetable
    {
        public override bool PlayerChoosesTarget
        {
            get
            {
                return true;
            }
        }

        public override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                canTargetItems = false,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => TargetValidator(x.Thing))
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
            yield break;
        }

        public bool TargetValidator(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                DefModExt_Xenotype modExt = parent.def.GetModExtension<DefModExt_Xenotype>();
                if(modExt != null)
                {
                    if(pawn.genes.Xenotype == modExt.xenotype)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
