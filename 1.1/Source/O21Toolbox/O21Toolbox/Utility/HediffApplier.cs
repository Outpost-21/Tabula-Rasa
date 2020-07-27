using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Projectiles;

namespace O21Toolbox.Utility
{
    public static class HediffApplier
    {
        public static void ApplyHediff(Thing hitThing, DefModExt_HediffApplier modExt)
        {
			Pawn pawn;
			if (modExt != null && hitThing != null)
			{
				pawn = (hitThing as Pawn);
				if (pawn != null)
				{
					float value = Rand.Value;
					if (modExt.chanceToApply >= value)
					{
						Pawn_HealthTracker health = pawn.health;
						Hediff hediff;
						if (health != null)
						{
							HediffSet hediffSet = health.hediffSet;
							hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(modExt.hediff, false) : null);
							if (hediff != null)
							{
								hediff.Severity += modExt.severityIncreasePerShot;
							}
							else
							{
								BodyPartRecord partRecord = null;
								Hediff hediffNew = HediffMaker.MakeHediff(modExt.hediff, pawn, null);
								hediffNew.Severity = modExt.initialSeverity;
								pawn.health.AddHediff(hediffNew, partRecord, null, null);
							}
						}
						float num = Rand.Range(0.99f, 0.999f);
					}
				}
				if (hitThing != null && hitThing.def.defName.Contains("Corpse_"))
				{
					hitThing.Destroy();
				}
			}
		}
    }
}
