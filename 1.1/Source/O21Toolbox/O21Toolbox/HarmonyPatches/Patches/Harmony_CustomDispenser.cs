using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.CustomDispenser;
using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_CustomDispenser
	{
		private static bool allowForbidden;

		private static bool allowDispenserFull;

		private static Pawn getter;

		private static Pawn eater;

		private static bool allowSociallyImproper;

		private static bool bestFoodSourceOnMap;

		public static int minimumHopperRefillThresholdPercent = 10;

		private static bool RepDel(Building_CustomDispenser dispenser)
		{
			return allowDispenserFull 
				&& getter.RaceProps.ToolUser 
				&& getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) 
				&& (dispenser.Faction == getter.Faction || dispenser.Faction == getter.HostFaction) 
				&& (allowForbidden || !dispenser.IsForbidden(getter)) 
				&& dispenser.powerComp.PowerOn 
				&& dispenser.InteractionCell.Standable(dispenser.Map) 
				&& CustomDispenserUtility.IsFoodSourceOnMapSociallyProper(dispenser, getter, eater, allowSociallyImproper) 
				&& !getter.IsWildMan() 
				&& dispenser.CanDispenseNow
				&& getter.Map.reachability.CanReachNonLocal(getter.Position, new TargetInfo(dispenser.InteractionCell, dispenser.Map, false), PathEndMode.OnCell, TraverseParms.For(getter, Danger.Some, TraverseMode.ByPawn, false));
		}

		[HarmonyPatch(typeof(ThingDef), "get_IsFoodDispenser")]
		private static class Building
		{
			[HarmonyPrefix]
			private static bool IsFoodDispenserPrefix(ThingDef __instance, ref bool __result)
			{
				if (__instance.thingClass == typeof(Building_CustomDispenser))
				{
					__result = false;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(FoodUtility), "BestFoodSourceOnMap")]
		[StaticConstructorOnStartup]
		private static class Patch_BestFoodSourceOnMap
		{
			private static void Prefix(ref Pawn getter, ref Pawn eater, ref bool allowDispenserFull, ref bool allowForbidden, ref bool allowSociallyImproper)
			{
				bestFoodSourceOnMap = true;
				Harmony_CustomDispenser.getter = getter;
				Harmony_CustomDispenser.eater = eater;
				Harmony_CustomDispenser.allowDispenserFull = allowDispenserFull;
				Harmony_CustomDispenser.allowForbidden = allowForbidden;
				Harmony_CustomDispenser.allowSociallyImproper = allowSociallyImproper;
			}

			private static void Postfix()
			{
				bestFoodSourceOnMap = false;
			}
		}

		[HarmonyPatch(typeof(FoodUtility), "FoodOptimality")]
		private static class Patch_FoodOptimality
		{
			private static bool Prefix(ref ThingDef foodDef, ref float __result)
			{
				if (foodDef == null)
				{
					__result = -9999999f;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(FoodUtility), "GetFinalIngestibleDef")]
		private static class Patch_GetFinalIngestibleDef
		{
			private static bool Prefix(ref Thing foodSource, ref ThingDef __result)
			{
				if (foodSource is Building_CustomDispenser && bestFoodSourceOnMap)
				{
					Building_CustomDispenser dispenser = (Building_CustomDispenser)foodSource;
					__result = dispenser.DispensableThing;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
		private static class Patch_Includes
		{
			private static bool Prefix(ref ThingRequestGroup group, ref ThingDef def, ref bool __result)
			{
				if ((group == ThingRequestGroup.FoodSource || group == ThingRequestGroup.FoodSourceNotPlantOrTree) && def.thingClass == typeof(Building_CustomDispenser))
				{
					__result = true;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(JobDriver_FoodDeliver), "GetReport")]
		private static class Patch_JobDriver_FoodDeliver_GetReport
		{
			private static void Postfix(JobDriver_FoodDeliver __instance, ref string __result)
			{
				if (__instance.job.GetTarget(TargetIndex.A).Thing is Building_CustomDispenser && (Pawn)__instance.job.targetB.Thing != null)
				{
					__result = __instance.job.def.reportString.Replace("TargetA", "ReplicatedMeal".Translate()).Replace("TargetB", __instance.job.targetB.Thing.LabelShort);
				}
			}
		}

		[HarmonyPatch(typeof(JobDriver_FoodFeedPatient), "GetReport")]
		private static class Patch_JobDriver_FoodFeedPatient_GetReport
		{
			private static void Postfix(JobDriver_FoodFeedPatient __instance, ref string __result)
			{
				if (__instance.job.GetTarget(TargetIndex.A).Thing is Building_CustomDispenser && (Pawn)__instance.job.targetB.Thing != null)
				{
					__result = __instance.job.def.reportString.Replace("TargetA", "ReplicatedMeal".Translate()).Replace("TargetB", __instance.job.targetB.Thing.LabelShort);
				}
			}
		}

		//[HarmonyPatch(typeof(JobDriver_Ingest), "GetReport")]
		//private static class Patch_JobDriver_Ingest_GetReport
		//{
		//	private static void Postfix(JobDriver_Ingest __instance, ref string __result)
		//	{
		//		if (__instance.)
		//		{
		//			Building_CustomDispenser dispenser = (Building_CustomDispenser)__instance.job.GetTarget(TargetIndex.A).Thing;
		//			if (dispenser != null)
		//			{
		//				__result = __instance.job.def.reportString.Replace("TargetA", dispenser.DispensableThing.label);
		//				return;
		//			}
		//			__result = __instance.job.def.reportString.Replace("TargetA", __instance.job.GetTarget(TargetIndex.A).Thing.Label);
		//		}
		//	}
		//}

		[HarmonyPatch(typeof(FoodUtility), "SpawnedFoodSearchInnerScan")]
		private static class Patch_SpawnedFoodSearchInnerScan
		{
			private static bool Prefix(ref Predicate<Thing> validator)
			{
				Predicate<Thing> malidator = validator;
				Predicate<Thing> predicate = delegate (Thing x)
				{
					Building_CustomDispenser t;
					if ((t = (x as Building_CustomDispenser)) == null)
					{
						return malidator(x);
					}
					return RepDel(t);
				};
				validator = predicate;
				return true;
			}
		}

		[HarmonyPatch(typeof(Toils_Ingest), "TakeMealFromDispenser")]
		private static class Patch_TakeMealFromDispenser
		{
			private static bool Prefix(ref TargetIndex ind, ref Pawn eater, ref Toil __result)
			{
				if (eater.jobs.curJob.GetTarget(ind).Thing is Building_CustomDispenser)
				{
					TargetIndex windex = ind;
					Toil toil = new Toil();
					toil.initAction = delegate ()
					{
						Pawn actor = toil.actor;
						Job curJob = actor.jobs.curJob;
						Building_CustomDispenser customDispenser = (Building_CustomDispenser)curJob.GetTarget(windex).Thing;
						Pawn eater2 = actor;
						Pawn pawn;
						if ((pawn = (curJob.GetTarget(TargetIndex.B).Thing as Pawn)) != null)
						{
							eater2 = pawn;
						}
						Thing thing = customDispenser.TryDispenseThing(eater2, actor);
						if (thing == null)
						{
							actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
							return;
						}
						actor.carryTracker.TryStartCarry(thing);
						actor.CurJob.SetTarget(windex, actor.carryTracker.CarriedThing);
					};
					toil.FailOnCannotTouch(ind, PathEndMode.Touch);
					toil.defaultCompleteMode = ToilCompleteMode.Delay;
					toil.defaultDuration = Building_NutrientPasteDispenser.CollectDuration;
					__result = toil;
					return false;
				}
				return true;
			}
		}
	}
}
