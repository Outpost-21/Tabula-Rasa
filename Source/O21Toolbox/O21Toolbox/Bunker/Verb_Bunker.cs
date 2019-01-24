using System;
using System.Collections.Generic;
using Verse;

namespace O21Toolbox.Bunker
{
    public class Verb_Bunker : Verb_LaunchProjectile
    {
        private Building_Bunker bunker;

        private List<Verb> verbsList;
        
        public override void Reset()
        {
            base.Reset();
            this.bunker = (Building_Bunker)this.caster;
        }
        
        public void ResetVerb()
        {
            bool flag = this.bunker == null;
            if (flag)
            {
                this.bunker = (Building_Bunker)this.caster;
            }
            foreach (Pawn pawn in this.bunker.GetInner().InnerListForReading)
            {
                bool flag2 = pawn.TryGetAttackVerb(this.currentTarget.Thing, false) != null;
                if (flag2)
                {
                    pawn.TryGetAttackVerb(this.currentTarget.Thing, false).caster = pawn;
                }
            }
        }
        
        protected override bool TryCastShot()
        {
            this.verbsList = new List<Verb>();
            bool flag = this.bunker == null;
            if (flag)
            {
                this.bunker = (Building_Bunker)this.caster;
            }
            foreach (Pawn pawn in this.bunker.GetInner().InnerListForReading)
            {
                bool flag2 = pawn.TryGetAttackVerb(this.currentTarget.Thing, false) != null;
                if (flag2)
                {
                    this.verbsList.Add(pawn.TryGetAttackVerb(this.currentTarget.Thing, false));
                }
            }
            foreach (Verb verb in this.verbsList)
            {
                verb.caster = this.caster;
                verb.TryStartCastOn(this.currentTarget, false, true);
            }
            return true;
        }
    }
}
