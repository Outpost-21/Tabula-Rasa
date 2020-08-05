using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Abilities
{
    public class JobDriver_CastAbilitySelf : JobDriver
    {
        public AbilityContext Context => job.count == 1 ? AbilityContext.Player : AbilityContext.AI;

        private List<Comp_AbilityUser> CompAbilityUsers
        {
            get
            {
                var results = new List<Comp_AbilityUser>();
                var allCompAbilityUsers = pawn.GetComps<Comp_AbilityUser>();
                if (allCompAbilityUsers.TryRandomElement(out var comp))
                    foreach (var compy in allCompAbilityUsers)
                        results.Add(compy);
                return results;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);

            var verb = pawn.CurJob.verbToUse as Verb_UseAbility;
            Find.Targeter.targetingSource = verb;
            yield return Toils_Combat.CastVerb(TargetIndex.A, false);
            yield return new Toil
            {
                initAction = delegate { verb.Ability.PostAbilityAttempt(); },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return new Toil
            {
                initAction = delegate
                {
                    if (verb.UseAbilityProps.isViolent)
                    {
                        JobDriver_CastAbilityVerb.CheckForAutoAttack(this.pawn);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
