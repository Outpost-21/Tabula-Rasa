using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.Needs;
using O21Toolbox.ArtificialPawn;
using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(FoodUtility), "WillIngestStackCountOf")]
    public class CompatPatch_WillIngestStackCountOf
    {
        [HarmonyPrefix]
        public static bool Prefix(int __result, ref Pawn ingester, ref ThingDef def)
        {
            if (ingester == null)
                return true;

            bool haveNeed = ingester?.needs.TryGetNeed(NeedDefOf.Food) != null;

            if (!haveNeed)
            {
                __result = 0;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(RecordWorker_TimeInBedForMedicalReasons), "ShouldMeasureTimeNow")]
    public class CompatPatch_ShouldMeasureTimeNow
    {
        [HarmonyPrefix]
        public static bool Prefix(bool __result, ref Pawn pawn)
        {
            if (pawn == null)
                return true;

            bool haveNeed = pawn?.needs.TryGetNeed(NeedDefOf.Rest) != null;

            if (!haveNeed)
            {
                __result = pawn.InBed() && (HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn) || (HealthAIUtility.ShouldSeekMedicalRest(pawn) && pawn.CurJob.restUntilHealed)); ;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDeadFromRequiredCapacity")]
    public class CompatPatch_ShouldBeDeadFromRequiredCapacity
    {
        public static FieldInfo int_Pawn_HealthTracker_GetPawn = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_HealthTracker __instance, ref PawnCapacityDef __result)
        {
            Pawn pawn = Pawn_HealthTracker_GetPawn(__instance);

            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                List<PawnCapacityDef> allDefsListForReading = DefDatabase<PawnCapacityDef>.AllDefsListForReading;
                for (int i = 0; i < allDefsListForReading.Count; i++)
                {
                    PawnCapacityDef pawnCapacityDef = allDefsListForReading[i];
                    if (allDefsListForReading[i] == PawnCapacityDefOf.Consciousness && !__instance.capacities.CapableOf(pawnCapacityDef))
                    {
                        __result = pawnCapacityDef;
                        return false;
                    }
                }

                __result = null;
                return false;
            }

            return true;
        }

        public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
        {
            return (Pawn)int_Pawn_HealthTracker_GetPawn.GetValue(instance);
        }
    }

    [HarmonyPatch(typeof(HediffSet), "CalculatePain")]
    public class CompatPatch_CalculatePain
    {
        [HarmonyPrefix]
        public static bool Prefix(ref HediffSet __instance, ref float __result)
        {
            if (__instance.pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = 0f;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(RestUtility), "TimetablePreventsLayDown")]
    public class CompatPatch_TimetablePreventsLayDown
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = pawn.timetable != null && !pawn.timetable.CurrentAssignment.allowRest;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(GatheringsUtility), "ShouldGuestKeepAttendingGathering")]
    public class CompatPatch_ShouldGuestKeepAttendingGathering
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref Pawn p)
        {
            //Log.Message("Guest p=" + p?.ToString() ?? "null");
            if (p.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                //Log.Message("Guest (Mechanical) p=" + p.ToString());
                __result = !p.Downed &&
                    p.health.hediffSet.BleedRateTotal <= 0f &&
                    !p.health.hediffSet.HasTendableNonInjuryNonMissingPartHediff(false) &&
                    !p.InAggroMentalState && !p.IsPrisoner;
                return false;
            }

            //Log.Message("Guest NOT (Mechanical) p=" + p?.ToString() ?? "null");
            return true;
        }

    }

    [HarmonyPatch(typeof(JobGiver_EatInGatheringArea), "TryGiveJob")]
    public class CompatPatch_EatInGatheringAreaTryGiveJob
    {
        [HarmonyPrefix]
        public static bool Prefix(ref JobGiver_EatInGatheringArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(JobGiver_GetJoy), "TryGiveJob")]
    public class CompatPatch_GetJoyTryGiveJob
    {
        [HarmonyPrefix]
        public static bool Prefix(ref JobGiver_EatInGatheringArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "SocialFightChance")]
    public class CompatPatch_SocialFightChance
    {
        public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_InteractionsTracker __instance, ref float __result, ref InteractionDef interaction, ref Pawn initiator)
        {
            int_Pawn_InteractionsTracker_GetPawn = typeof(Pawn_InteractionsTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if ((pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize) || (initiator.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties propertiesTwo && !propertiesTwo.canSocialize))
            {
                __result = 0f;
                return false;
            }

            return true;
        }

        public static Pawn Pawn_InteractionsTracker_GetPawn(Pawn_InteractionsTracker instance)
        {
            return (Pawn)int_Pawn_InteractionsTracker_GetPawn.GetValue(instance);
        }

    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "InteractionsTrackerTick")]
    public class CompatPatch_InteractionsTrackerTick
    {
        public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_InteractionsTracker __instance)
        {
            int_Pawn_InteractionsTracker_GetPawn = typeof(Pawn_InteractionsTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }

        public static Pawn Pawn_InteractionsTracker_GetPawn(Pawn_InteractionsTracker instance)
        {
            return (Pawn)int_Pawn_InteractionsTracker_GetPawn.GetValue(instance);
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "CanInteractNowWith")]
    public class CompatPatch_CanInteractNowWith
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn_InteractionsTracker __instance, ref bool __result, ref Pawn recipient)
        {
            //Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (recipient.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(InteractionUtility), "CanInitiateInteraction")]
    public class CompatPatch_CanInitiateInteraction
    {
        [HarmonyPrefix]
        public static bool CompatPatch_CanDoInteraction(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(InteractionUtility), "CanReceiveInteraction")]
    public class CompatPatch_CanReceiveInteraction
    {
        [HarmonyPrefix]
        public static bool CompatPatch_CanDoInteraction(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
    public class CompatPatch_AppendThoughts_ForHumanlike
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn victim)
        {
            if (victim.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.colonyCaresIfDead)
            {
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(InspirationHandler), "InspirationHandlerTick")]
    public class CompatPatch_InspirationHandlerTick
    {
        [HarmonyPrefix]
        public static bool Prefix(ref InspirationHandler __instance)
        {
            if (__instance.pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils")]
    public class CompatPatch_VomitJob
    {
        [HarmonyPrefix]
        public static bool Prefix(ref JobDriver_Vomit __instance, ref IEnumerable<Toil> __result)
        {
            Pawn pawn = __instance.pawn;

            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                JobDriver_Vomit instance = __instance;

                List<Toil> toils = new List<Toil>();
                toils.Add(new Toil()
                {
                    initAction = delegate ()
                    {
                        instance.pawn.jobs.StopAll();
                    }
                });

                __result = toils;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(Alert_Boredom), "GetReport")]
    public class CompatPatch_Boredom_GetReport
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Alert_Boredom __instance, ref AlertReport __result)
        {
            IEnumerable<Pawn> culprits = null;
            CompatPatch_BoredPawns(ref culprits);

            __result = AlertReport.CulpritsAre(culprits.ToList());
            return false;
        }
        public static bool CompatPatch_BoredPawns(ref IEnumerable<Pawn> __result)
        {
            //Log.Message("CompatPatch_BoredPawns Alert");

            List<Pawn> legiblePawns = new List<Pawn>();

            foreach (Pawn p in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                //Log.Message("Pawn=" + p.Label);
                if (p.needs.joy != null && (p.needs.joy.CurLevelPercentage < 0.24000001f || p.GetTimeAssignment() == TimeAssignmentDefOf.Joy))
                {
                    if (p.needs.joy.tolerances.BoredOfAllAvailableJoyKinds(p))
                    {
                        legiblePawns.Add(p);
                    }
                }
            }

            /*if(legiblePawns.Count > 0)
            {
                __result = legiblePawns;
            }
            else
            {
                __result = null;
            }*/

            __result = legiblePawns;

            //NO Original method
            return false;
        }

    }

    [HarmonyPatch(typeof(Toils_Tend), "FinalizeTend")]
    public class Patch_Toils_Tend_FinalizeTend
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Toil __result, Pawn patient)
        {
            if (patient.def.HasModExtension<ArtificialPawnProperties>())
            {
                Toil toil = new Toil();
                toil.initAction = delegate
                {
                    Pawn actor = toil.actor;

                    Thing repairParts = (Thing)actor.CurJob.targetB.Thing;

                    if (patient.def.GetModExtension<ArtificialPawnProperties>().repairParts != null && !patient.def.GetModExtension<ArtificialPawnProperties>().repairParts.Contains(actor.CurJob.targetB.Thing.def))
                    {
                        repairParts = null;
                    }

                    //Experience
                    float num = (!patient.RaceProps.Animal) ? 500f : 175f;
                    float num2 = RimWorld.ThingDefOf.MedicineIndustrial.MedicineTendXpGainFactor;
                    actor.skills.Learn(SkillDefOf.Crafting, num * num2, false);

                    //Tending
                    //TendUtility.DoTend(actor, patient, medicine);
                    DroidUtility.DoTend(actor, patient, repairParts);

                    if (repairParts != null && repairParts.Destroyed)
                    {
                        actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
                    }
                    if (toil.actor.CurJob.endAfterTendedOnce)
                    {
                        actor.jobs.EndCurrentJob(JobCondition.Succeeded, true);
                    }
                };
                toil.defaultCompleteMode = ToilCompleteMode.Instant;
                __result = toil;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(HealthAIUtility), "FindBestMedicine")]
    public class Patch_HealthAIUtility_FindBestMedicine
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Thing __result, Pawn healer, Pawn patient)
        {
            if (patient.def.HasModExtension<ArtificialPawnProperties>())
            {
                Thing result;
                if (patient.playerSettings == null || patient.playerSettings.medCare <= MedicalCareCategory.NoMeds)
                {
                    result = null;
                }
                else if (Medicine.GetMedicineCountToFullyHeal(patient) <= 0)
                {
                    result = null;
                }
                else
                {
                    Predicate<Thing> predicate = (Thing m) => !m.IsForbidden(healer) && patient.playerSettings.medCare.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1, null, false) && m.def.GetModExtension<DefModExtension_RepairPartsProps>() != null;
                    Func<Thing, float> priorityGetter = delegate (Thing t)
                    {
                        DefModExtension_RepairPartsProps repairParts = t.def.GetModExtension<DefModExtension_RepairPartsProps>();
                        if (repairParts == null)
                            return 0f;

                        return repairParts.repairPotency;
                    };
                    IntVec3 position = patient.Position;
                    Map map = patient.Map;
                    List<Thing> searchSet = patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableEver);
                    PathEndMode peMode = PathEndMode.ClosestTouch;
                    TraverseParms traverseParams = TraverseParms.For(healer, Danger.Deadly, TraverseMode.ByPawn, false);
                    Predicate<Thing> validator = predicate;
                    result = GenClosest.ClosestThing_Global_Reachable(position, map, searchSet, peMode, traverseParams, 9999f, validator, priorityGetter);
                }

                __result = result;
                return false;
            }

            return true;
        }

    }
}
