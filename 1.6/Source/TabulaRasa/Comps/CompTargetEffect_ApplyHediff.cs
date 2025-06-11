using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa
{
    public class CompTargetEffect_ApplyHediff : CompTargetEffect
	{
		public CompProperties_TargetEffectApplyHediff Props => (CompProperties_TargetEffectApplyHediff)props;

		public override void DoEffectOn(Pawn user, Thing target)
		{
			if (user.IsColonistPlayerControlled && user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly))
			{
				Job job = JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UseEffectApplyHediff, target, parent);
				job.count = 1;
				JobDriver_ApplyHediff driver = (JobDriver_ApplyHediff)job.GetCachedDriver(user);
				driver.hediffDef = Props.hediff;
				user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}
		}
	}
}
