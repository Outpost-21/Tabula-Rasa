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
	//TODO: Remove next RW update
	public class JobGiver_IntelligentAnimal : JobGiver_Work
    {
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			if (pawn.workSettings == null)
			{
				return ThinkResult.NoJob;
			}
			if (emergency && pawn.mindState.priorityWork.IsPrioritized)
			{
				List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkGiver.workType.workGiversByPriority;
				for (int i = 0; i < workGiversByPriority.Count; i++)
				{
					WorkGiver worker = workGiversByPriority[i].Worker;
					if (WorkGiversRelated(pawn.mindState.priorityWork.WorkGiver, worker.def))
					{
						Job job = GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
						if (job != null)
						{
							job.playerForced = true;
							return new ThinkResult(job, this, workGiversByPriority[i].tagToGive);
						}
					}
				}
				pawn.mindState.priorityWork.Clear();
			}
			List<WorkGiver> list = ((!emergency) ? pawn.workSettings.WorkGiversInOrderNormal : pawn.workSettings.WorkGiversInOrderEmergency);
			int num = -999;
			TargetInfo bestTargetOfLastPriority = TargetInfo.Invalid;
			WorkGiver_Scanner scannerWhoProvidedTarget = null;
			WorkGiver_Scanner scanner;
			IntVec3 pawnPosition;
			float closestDistSquared;
			float bestPriority;
			bool prioritized;
			bool allowUnreachable;
			Danger maxPathDanger;
			for (int j = 0; j < list.Count; j++)
			{
				WorkGiver workGiver = list[j];
				if (workGiver.def.priorityInType != num && bestTargetOfLastPriority.IsValid)
				{
					break;
				}
				if (!PawnCanUseWorkGiver(pawn, workGiver))
				{
					continue;
				}
				try
				{
					Job job2 = workGiver.NonScanJob(pawn);
					if (job2 != null)
					{
						return new ThinkResult(job2, this, list[j].def.tagToGive);
					}
					scanner = workGiver as WorkGiver_Scanner;
					if (scanner != null)
					{
						if (scanner.def.scanThings)
						{
							IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
							Thing thing;
							if (scanner.Prioritized)
							{
								IEnumerable<Thing> enumerable2 = enumerable;
								if (enumerable2 == null)
								{
									enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = ((!scanner.AllowUnreachable) ? GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x)) : GenClosest.ClosestThing_Global(pawn.Position, enumerable2, 99999f, validator, (Thing x) => scanner.GetPriority(pawn, x)));
							}
							else if (scanner.AllowUnreachable)
							{
								IEnumerable<Thing> enumerable3 = enumerable;
								if (enumerable3 == null)
								{
									enumerable3 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = GenClosest.ClosestThing_Global(pawn.Position, enumerable3, 99999f, validator);
							}
							else
							{
								thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, enumerable != null);
							}
							if (thing != null)
							{
								bestTargetOfLastPriority = thing;
								scannerWhoProvidedTarget = scanner;
							}
						}
						if (scanner.def.scanCells)
						{
							pawnPosition = pawn.Position;
							closestDistSquared = 99999f;
							bestPriority = float.MinValue;
							prioritized = scanner.Prioritized;
							allowUnreachable = scanner.AllowUnreachable;
							maxPathDanger = scanner.MaxPathDanger(pawn);
							IEnumerable<IntVec3> enumerable4 = scanner.PotentialWorkCellsGlobal(pawn);
							if (enumerable4 is IList<IntVec3> list2)
							{
								for (int k = 0; k < list2.Count; k++)
								{
									ProcessCell(list2[k]);
								}
							}
							else
							{
								foreach (IntVec3 item in enumerable4)
								{
									ProcessCell(item);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogUtil.LogError($"{pawn} threw exception in WorkGiver {workGiver.def.defName}: {ex}");
				}
				if (bestTargetOfLastPriority.IsValid)
				{
					Job job3 = ((!bestTargetOfLastPriority.HasThing) ? scannerWhoProvidedTarget.JobOnCell(pawn, bestTargetOfLastPriority.Cell) : scannerWhoProvidedTarget.JobOnThing(pawn, bestTargetOfLastPriority.Thing));
					if (job3 != null)
					{
						job3.workGiverDef = scannerWhoProvidedTarget.def;
						return new ThinkResult(job3, this, list[j].def.tagToGive);
					}
					LogUtil.LogError($"{scannerWhoProvidedTarget} provided target {bestTargetOfLastPriority} but yielded no actual job for pawn {pawn}. The CanGiveJob and JobOnX methods may not be synchronized.");
				}
				num = workGiver.def.priorityInType;
			}
			return ThinkResult.NoJob;
			void ProcessCell(IntVec3 c)
			{
				bool flag = false;
				float num2 = (c - pawnPosition).LengthHorizontalSquared;
				float num3 = 0f;
				if (prioritized)
				{
					if (!c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
					{
						if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
						{
							return;
						}
						num3 = scanner.GetPriority(pawn, c);
						if (num3 > bestPriority || (num3 == bestPriority && num2 < closestDistSquared))
						{
							flag = true;
						}
					}
				}
				else if (num2 < closestDistSquared && !c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
				{
					if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
					{
						return;
					}
					flag = true;
				}
				if (flag)
				{
					bestTargetOfLastPriority = new TargetInfo(c, pawn.Map);
					scannerWhoProvidedTarget = scanner;
					closestDistSquared = num2;
					bestPriority = num3;
				}
			}
			bool validator(Thing t)
			{
				return !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
			}
		}

		public new bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			if (!giver.def.nonColonistsCanDo && !(pawn.Faction?.IsPlayer).Value)
			{
				return false;
			}
			if (pawn.WorkTagIsDisabled(giver.def.workTags))
			{
				return false;
			}
			if (giver.ShouldSkip(pawn))
			{
				return false;
			}
			if (giver.MissingRequiredCapacity(pawn) != null)
			{
				return false;
			}
			return true;
		}

		public new bool WorkGiversRelated(WorkGiverDef current, WorkGiverDef next)
		{
			if (next != WorkGiverDefOf.Repair || current == WorkGiverDefOf.Repair)
			{
				return next.doesSmoothing == current.doesSmoothing;
			}
			return false;
		}

		public new Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
		{
			if (!PawnCanUseWorkGiver(pawn, giver))
			{
				return null;
			}
			try
			{
				Job job = giver.NonScanJob(pawn);
				if (job != null)
				{
					return job;
				}
				WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
				if (scanner != null)
				{
					if (giver.def.scanThings)
					{
						List<Thing> thingList = cell.GetThingList(pawn.Map);
						for (int i = 0; i < thingList.Count; i++)
						{
							Thing t2 = thingList[i];
							if (scanner.PotentialWorkThingRequest.Accepts(t2) && predicate(t2))
							{
								Job job2 = scanner.JobOnThing(pawn, t2);
								if (job2 != null)
								{
									job2.workGiverDef = giver.def;
								}
								return job2;
							}
						}
					}
					if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell))
					{
						Job job3 = scanner.JobOnCell(pawn, cell);
						if (job3 != null)
						{
							job3.workGiverDef = giver.def;
						}
						return job3;
					}
				}
				bool predicate(Thing t)
				{
					return !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
				}
			}
			catch (Exception ex)
			{
				LogUtil.LogError($"{pawn} threw exception in GiverTryGiveJobTargeted on WorkGiver {giver.def.defName}: {ex}");
			}
			return null;
		}
	}
}
