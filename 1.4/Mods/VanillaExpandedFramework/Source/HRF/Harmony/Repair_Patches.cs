using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{

    [HarmonyPatch(typeof(WorkGiver_FixBrokenDownBuilding), "HasJobOnThing")]
    public static class Patch_HasJobOnThing
    {
        private static bool Prefix(ref bool __result, Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.HasEnoughResourceToRepair(out _))
            {
                HRFLog.Message("Has enough resource to repair");
                __result = HasJobOnThing(pawn, t, forced);
                return false;
            }
            return true;
        }

        public static bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building building = t as Building;
            if (building == null)
            {
                return false;
            }
            if (!building.def.building.repairable)
            {
                return false;
            }
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            if (!t.IsBrokenDown())
            {
                return false;
            }
            if (t.IsForbidden(pawn))
            {
                return false;
            }
            if (pawn.Faction == Faction.OfPlayerSilentFail && !pawn.Map.areaManager.Home[t.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
                return false;
            }
            if (!pawn.CanReserve(building, 1, -1, null, forced))
            {
                return false;
            }
            if (pawn.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }
            if (building.IsBurning())
            {
                return false;
            }
            return true;
        }

        public static bool HasEnoughResourceToRepair(this Pawn repairer, out (HediffResource, RepairProperties) toConsume)
        {
            foreach (var hediff in repairer.health.hediffSet.hediffs)
            {
                if (hediff is HediffResource hediffResource && hediffResource.CurStage is HediffStageResource hediffStageResource && hediffStageResource.repairProperties != null)
                {
                    hediffResource = hediffStageResource.repairProperties.hediffResource != null
                        ? repairer.health.hediffSet.GetFirstHediffOfDef(hediffStageResource.repairProperties.hediffResource) as HediffResource
                        : hediffResource;

                    if (hediffResource != null)
                    {
                        bool canRepair = hediffResource.ResourceAmount >= hediffStageResource.repairProperties.resourceOnRepair;
                        if (canRepair)
                        {
                            toConsume = (hediffResource, hediffStageResource.repairProperties);
                            return true;
                        }
                    }
                }
            }
            toConsume = default;
            return false;
        }
    }
    [HarmonyPatch(typeof(JobDriver_FixBrokenDownBuilding), "TryMakePreToilReservations")]
    public static class Patch_TryMakePreToilReservations
    {
        private static bool Prefix(ref bool __result, JobDriver_FixBrokenDownBuilding __instance, bool errorOnFailed)
        {
            if (__instance.pawn.HasEnoughResourceToRepair(out _))
            {
                HRFLog.Message("Has enough resource to repair");
                __result = __instance.pawn.Reserve(__instance.job.GetTarget(TargetIndex.A).Thing, __instance.job, 1, -1, null, errorOnFailed);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobDriver_FixBrokenDownBuilding), "MakeNewToils")]
    public static class Patch_MakeNewToils
    {
        private static bool Prefix(ref IEnumerable<Toil> __result, JobDriver_FixBrokenDownBuilding __instance)
        {
            if (__instance.pawn.HasEnoughResourceToRepair(out _))
            {
                HRFLog.Message("Has enough resource to repair");
                __result = MakeNewToils(__instance).ToList();
                return false;
            }
            return true;
        }

        private static IEnumerable<Toil> MakeNewToils(JobDriver_FixBrokenDownBuilding __instance)
        {
            __instance.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(1000);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.WithEffect(__instance.job.GetTarget(TargetIndex.A).Thing.def.repairEffect, TargetIndex.A);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.activeSkill = () => SkillDefOf.Construction;
            yield return toil;
            Toil toil2 = new Toil();
            toil2.initAction = delegate
            {
                if (!__instance.pawn.HasEnoughResourceToRepair(out var repairData))
                {
                    MoteMaker.ThrowText((__instance.pawn.DrawPos + __instance.job.GetTarget(TargetIndex.A).Thing.DrawPos) / 2f, __instance.pawn.Map,
                        "TextMote_FixBrokenDownBuildingFail".Translate() + " - " + "ART.NotEnoughResource".Translate(), 3.65f);
                }
                else if (Rand.Value > __instance.pawn.GetStatValue(StatDefOf.FixBrokenDownBuildingSuccessChance))
                {
                    MoteMaker.ThrowText((__instance.pawn.DrawPos + __instance.job.GetTarget(TargetIndex.A).Thing.DrawPos) / 2f, __instance.pawn.Map,
                        "TextMote_FixBrokenDownBuildingFail".Translate(), 3.65f);
                }
                else
                {
                    __instance.job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompBreakdownable>().Notify_Repaired();
                    repairData.Item1.ChangeResourceAmount(-repairData.Item2.resourceOnRepair);
                }
            };
            yield return toil2;
        }
    }
}