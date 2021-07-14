using System;
using System.Collections.Generic;
using Verse;

namespace O21Toolbox.TurretsPlus
{
    public class Verb_Emplacement : Verb_Shoot
    {
        private Building_BunkerEmplacement bunker;
        
        public override void Reset()
        {
            base.Reset();
            this.bunker = (Building_BunkerEmplacement)this.caster;
        }
        
        public void ResetVerb()
        {
            bool flag = this.bunker == null;
            if (flag)
            {
                this.bunker = (Building_BunkerEmplacement)this.caster;
            }
            if (bunker.GetInner() != null)
            {
                bool flag2 = bunker.GetInner().TryGetAttackVerb(this.currentTarget.Thing, false) != null;
                if (flag2)
                {
                    pawn.TryGetAttackVerb(this.currentTarget.Thing, false).caster = pawn;
                }
            }
        }
        
        protected override bool TryCastShot()
        {
            bool flag = this.bunker == null;
            if (flag)
            {
                this.bunker = (Building_BunkerEmplacement)this.caster;
            }
            foreach (Pawn pawn in this.bunker.GetInner().InnerListForReading)
            {
                bool flag2 = pawn.TryGetAttackVerb(this.currentTarget.Thing, false) != null;
                if (flag2)
                {
                    this.verbss.Add(pawn.TryGetAttackVerb(this.currentTarget.Thing, false));
                }
            }
            foreach (Verb verb in this.verbss)
            {
                verb.caster = this.caster;
                verb.TryStartCastOn(this.currentTarget, false, true);
            }
            return true;
            bool flag = base.TryCastShot();
            if (flag && base.CasterIsPawn)
            {
                this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
