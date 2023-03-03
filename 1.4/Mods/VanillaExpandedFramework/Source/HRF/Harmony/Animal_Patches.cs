using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using VFECore;

namespace HRF
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "Interacted")]
    public static class Patch_Interacted
    {
        public static void Postfix(Pawn initiator, Pawn recipient)
        {
            if (initiator != null && recipient != null)
            {
                var comp = recipient.TryGetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (useProps.resourceOnTaming != 0)
                        {
                            Utils.AdjustResourceAmount(initiator, useProps.hediff, useProps.resourceOnTaming, useProps.addHediffIfMissing, null, null);
                        }
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(WorkGiver_Tame), "JobOnThing")]
    public static class Patch_WorkGiver_Tame_JobOnThing
    {
        public static bool Prefix(ref Job __result, WorkGiver_Tame __instance, Pawn pawn, Thing t, bool forced = false)
        {
            var pawn2 = t as Pawn;
            if (pawn2 != null)
            {
                var comp = pawn2.GetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (!pawn.CanUseIt(pawn2.Label, useProps, useProps.resourceOnTaming, useProps.cannotTameMessageKey, out var failMessage))
                        {
                            JobFailReason.Is(failMessage);
                            __result = null;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
    public static class TryInteractWith_Patch
    {
        public static void Postfix(bool __result, Pawn ___pawn, Pawn recipient, InteractionDef intDef)
        {
            if (__result && intDef == InteractionDefOf.TrainAttempt)
            {
                var comp = recipient.TryGetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (useProps.resourceOnTraining != 0)
                        {
                            Utils.AdjustResourceAmount(___pawn, useProps.hediff, useProps.resourceOnTraining, useProps.addHediffIfMissing, null, null);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Train), "JobOnThing")]
    public static class Patch_WorkGiver_Train_JobOnThing
    {
        public static bool Prefix(ref Job __result, WorkGiver_Tame __instance, Pawn pawn, Thing t, bool forced = false)
        {
            var pawn2 = t as Pawn;
            if (pawn2 != null)
            {
                var comp = pawn2.GetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (!pawn.CanUseIt(pawn2.Label, useProps, useProps.resourceOnTraining, useProps.cannotTrainMessageKey, out var failMessage))
                        {
                            JobFailReason.Is(failMessage);
                            __result = null;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(CompHasGatherableBodyResource), "Gathered")]
    public static class Gathered_Patch
    {
        public static void Postfix(CompHasGatherableBodyResource __instance, Pawn doer)
        {
            var pawnShearing = __instance.parent as Pawn;
            if (pawnShearing != null)
            {
                var comp = pawnShearing.TryGetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (useProps.resourceOnGather != 0)
                        {
                            Utils.AdjustResourceAmount(doer, useProps.hediff, useProps.resourceOnGather, useProps.addHediffIfMissing, null, null);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorkGiver_GatherAnimalBodyResources), "HasJobOnThing")]
    public static class Patch_WorkGiver_GatherAnimalBodyResources_HasJobOnThing
    {
        public static bool Prefix(ref bool __result, WorkGiver_Tame __instance, Pawn pawn, Thing t, bool forced = false)
        {
            var pawn2 = t as Pawn;
            if (pawn2 != null)
            {
                var comp = pawn2.GetComp<CompThingInUse>();
                if (comp != null)
                {
                    foreach (var useProps in comp.Props.useProperties)
                    {
                        if (!pawn.CanUseIt(pawn2.Label, useProps, useProps.resourceOnGather, useProps.cannotGatherMessageKey, out var failMessage))
                        {
                            JobFailReason.Is(failMessage);
                            __result = false;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }


    public class IngestionData
    {
        public HediffResource hediffResource;
        public float requiredResourceAmount;
        public float requiredNutrition;
    }
    [HarmonyPatch(typeof(WorkGiver_InteractAnimal), "HasFoodToInteractAnimal")]
    public static class Patch_HasFoodToInteractAnimal
    {
        public static void Postfix(ref bool __result, Pawn pawn, Pawn tamee)
        {
            if (!__result)
            {
                __result = pawn.CanFeedAnimalWithResource(tamee, out _);
            }
        }

        public static bool CanFeedAnimalWithResource(this Pawn pawn, Pawn tamee, out IngestionData ingestionData)
        {
            ingestionData = default;
            if (pawn?.health?.hediffSet?.hediffs != null)
            {
                foreach (var hediffResource in pawn.health.hediffSet.hediffs.OfType<HediffResource>())
                {
                    if (hediffResource.CurStage is HediffStageResource hediffStageResource && hediffStageResource.ingestibleProperties != null
                        && (hediffStageResource.ingestibleProperties.nutritionCategories & tamee.def.race.foodType) != 0)
                    {
                        ingestionData = new IngestionData();
                        ingestionData.hediffResource = hediffStageResource.ingestibleProperties.hediffResource != null
                            ? pawn.health.hediffSet.GetFirstHediffOfDef(hediffStageResource.ingestibleProperties.hediffResource) as HediffResource
                            : hediffResource;
                        var requiredNutrition = JobDriver_InteractAnimal.RequiredNutritionPerFeed(tamee);
                        var requiredResourceAmount = hediffStageResource.ingestibleProperties.resourcePerIngestion * requiredNutrition;
                        if (ingestionData.hediffResource != null && ingestionData.hediffResource.ResourceAmount >= requiredResourceAmount)
                        {
                            ingestionData.requiredResourceAmount = requiredResourceAmount;
                            ingestionData.requiredNutrition = requiredNutrition;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(JobDriver_InteractAnimal), "FeedToils")]
    public static class Patch_FeedToils
    {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> __result, JobDriver_InteractAnimal __instance)
        {
            var pawn = __instance.pawn;
            var tamee = __instance.job.targetA.Pawn;
            if (pawn.CanFeedAnimalWithResource(tamee, out _))
            {
                Toil toil = new Toil();
                toil.initAction = delegate
                {
                    __instance.feedNutritionLeft = JobDriver_InteractAnimal.RequiredNutritionPerFeed(tamee);
                };
                toil.defaultCompleteMode = ToilCompleteMode.Instant;
                yield return toil;
                Toil gotoAnimal = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
                yield return gotoAnimal;
                yield return FinalizeIngest(__instance, pawn, tamee);
                yield return Toils_Jump.JumpIf(gotoAnimal, () => __instance.feedNutritionLeft > 0f);
            }
            else
            {
                foreach (var r in __result)
                {
                    yield return r;
                }
            }
        }

        public static Toil FinalizeIngest(JobDriver_InteractAnimal __instance, Pawn pawn, Pawn ingester)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                if (pawn.CanFeedAnimalWithResource(ingester, out var ingestionData))
                {
                    __instance.feedNutritionLeft -= ingestionData.requiredNutrition;
                    if (__instance.feedNutritionLeft < 0.001f)
                    {
                        __instance.feedNutritionLeft = 0f;
                    }
                    PawnUtility.ForceWait(ingester, 270, pawn);
                    Pawn actor = toil.actor;
                    Job curJob = actor.jobs.curJob;
                    ingestionData.hediffResource.ChangeResourceAmount(-ingestionData.requiredResourceAmount);
                    if (!ingester.Dead)
                    {
                        ingester.needs.food.CurLevel += ingestionData.requiredNutrition;
                    }
                    ingester.records.AddTo(RecordDefOf.NutritionEaten, ingestionData.requiredNutrition);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            return toil;
        }
    }
}
