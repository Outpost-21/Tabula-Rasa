using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
	public class JobDriver_MaintainResourceBuilding : JobDriver
	{
		private const int MaintainTicks = 180;
		private CompMaintainableResourceBuilding compMaintainable;
		public CompMaintainableResourceBuilding CompMaintainable
		{
			get
			{
				if (compMaintainable == null)
				{
					compMaintainable = TargetA.Thing.TryGetComp<CompMaintainableResourceBuilding>();
				}
				return compMaintainable;
			}
		}
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => !this.CompMaintainable.CanMaintain(pawn, out string failReason));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil toil = Toils_General.Wait(180);
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			yield return toil;
			Toil maintain = new Toil();
			maintain.initAction = delegate
			{
				CompMaintainable.Maintained(pawn);
			};
			maintain.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return maintain;
		}
	}
}