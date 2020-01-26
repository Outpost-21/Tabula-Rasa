using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class Projectile_HediffApplier : Projectile
    {
        public DefModExt_HediffApplier modExt => this.def.GetModExtension<DefModExt_HediffApplier>();

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            Pawn pawn;
            bool flag = false;
            if (this.modExt != null && hitThing != null)
            {
                pawn = (hitThing as Pawn);
                flag = (pawn != null);
				if (flag)
				{
					float value = Rand.Value;
					if (this.modExt.chanceToApply >= value)
					{
						Pawn_HealthTracker health = pawn.health;
						Hediff hediff;
						if (health != null)
						{
							HediffSet hediffSet = health.hediffSet;
							hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(this.modExt.hediff, false) : null);
							if (hediff != null)
							{
								hediff.Severity += this.modExt.severityIncreasePerShot;
							}
							else
							{
								BodyPartRecord partRecord = null;
								Hediff hediffNew = HediffMaker.MakeHediff(this.modExt.hediff, pawn, null);
								hediffNew.Severity = this.modExt.initialSeverity;
								pawn.health.AddHediff(hediffNew, partRecord, null, null);
							}
						}
						float num = Rand.Range(0.99f, 0.999f);
					}
				}
				if(hitThing != null && hitThing.def.defName.Contains("Corspe_"))
				{
					hitThing.Destroy();
				}
			}
        }
    }
}
