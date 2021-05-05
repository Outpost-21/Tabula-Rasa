using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Jetpack
{
    public class JobDriver_JetpackRefuel : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, true);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			Pawn actor = base.GetActor();
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil refuel = Toils_General.Wait(180, TargetIndex.None);
			refuel.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			refuel.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			refuel.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			yield return refuel;
			yield return new Toil
			{
				initAction = delegate ()
				{
					int jetpackFuel = 0;
					int jetpackMax = 0;
					Pawn obj = actor;
					if (obj != null && obj.apparel.WornApparelCount == 0)
					{
						this.EndJobWith(JobCondition.Incompletable);
					}
					else
					{
						Apparel JetPack = null;
						List<Apparel> list = actor.apparel.WornApparel;
						for (int i = 0; i < list.Count; i++)
						{
							bool flag2 = list[i] is Apparel_Jetpack;
							if (flag2)
							{
								JetPack = list[i];
								break;
							}
						}
						if (JetPack == null)
						{
							this.EndJobWith(JobCondition.Incompletable);
						}
						else
						{
							if (JetPack is Apparel_Jetpack)
							{
								jetpackFuel = (JetPack as Apparel_Jetpack).comp.remainingCharges;
								jetpackMax = (JetPack as Apparel_Jetpack).comp.Props.maxCharges;
							}
							if (jetpackMax - jetpackFuel <= 0)
							{
								this.EndJobWith(JobCondition.Incompletable);
							}
							else
							{
								if (this.TargetThingA.stackCount > jetpackMax - jetpackFuel)
								{
									(JetPack as Apparel_Jetpack).comp.remainingCharges = jetpackMax;
									this.TargetThingA.stackCount -= jetpackMax - jetpackFuel;
									Messages.Message("Jetpack fully refueled", actor, MessageTypeDefOf.NeutralEvent, false);
									this.EndJobWith(JobCondition.Succeeded);
								}
								else
								{
									(JetPack as Apparel_Jetpack).comp.remainingCharges = jetpackFuel + this.TargetThingA.stackCount;
									Messages.Message("Jetpack partially refueled", actor, MessageTypeDefOf.NeutralEvent, false);
									this.TargetThingA.Destroy(DestroyMode.Vanish);
									this.EndJobWith(JobCondition.Succeeded);
								}
							}
						}
					}
				}
			};
			yield break;
		}
	}
}
