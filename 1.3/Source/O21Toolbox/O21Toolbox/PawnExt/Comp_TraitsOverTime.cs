using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class Comp_TraitsOverTime : ThingComp
    {
        public CompProperties_TraitsOverTime Props => (CompProperties_TraitsOverTime)props;

        public Pawn pawn => parent as Pawn;

        public int nextAttemptTimer = -1;

        public int CurrentTraitCount => pawn.story.traits.allTraits.Count();

        public override void CompTick()
        {
            base.CompTick();

            if(CurrentTraitCount < Props.maxTraits)
            {
                if(nextAttemptTimer <= 0)
                {
                    AddRandomTrait();
                    ResetTimer();
                }
            }
            nextAttemptTimer--;
        }

        public void AddRandomTrait()
        {
            if (!Props.traitWhitelist.NullOrEmpty())
            {
                bool flag = false;
                while (flag == false)
                {
                    Utility.TraitEntry selected = Props.traitWhitelist.RandomElementByWeight(x => x.chance);
                    TraitDef trait = DefDatabase<TraitDef>.GetNamed(selected.defName);
                    if (!pawn.story.traits.HasTrait(trait))
                    {
                        pawn.story.traits.GainTrait(new Trait(trait, selected.degree));
                        flag = true;
                    }
                }
            }
            else if(!Props.traitBlacklist.NullOrEmpty())
            {
                new NotImplementedException();
            }
        }

        public void ResetTimer()
        {
            nextAttemptTimer = Props.timeBetweenTraits.RandomInRange;
        }
    }
}
