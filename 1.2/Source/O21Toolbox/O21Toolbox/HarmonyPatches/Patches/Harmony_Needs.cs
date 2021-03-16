using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.ArtificialPawn;
using O21Toolbox.Needs;

namespace O21Toolbox.HarmonyPatches
{
    public class Harmony_Needs
    {
        /// <summary>
        /// For internal use: Field for getting the pawn.
        /// </summary>
        //public static FieldInfo int_Pawn_NeedsTracker_GetPawn;
        //public static FieldInfo int_PawnRenderer_GetPawn;
        //public static FieldInfo int_Need_Food_Starving_GetPawn;
        //public static FieldInfo int_ConditionalPercentageNeed_need;
        //public static FieldInfo int_Pawn_HealthTracker_GetPawn;
        //public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

        public static NeedDef Need_Bladder;
        public static NeedDef Need_Hygiene;

        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {
            //Try get needs.
            Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");
        }

        [HarmonyPatch(typeof(WorkGiver_Warden_Feed), "JobOnThing")]
        public static class Patch_WardenFeed_JobOnThing
        {
            [HarmonyPrefix]
            public static bool Prefix(ref WorkGiver_Warden_Feed __instance, Pawn pawn, Thing t, bool forced)
            {
                if(t is Pawn prisoner && !prisoner.def.race.EatsFood)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "JobOnThing")]
        public static class Patch_WardenDeliverFood_JobOnThing
        {
            [HarmonyPrefix]
            public static bool Prefix(ref WorkGiver_Warden_DeliverFood __instance, Pawn pawn, Thing t, bool forced, ref Job __result)
            {
                if (t is Pawn prisoner && !prisoner.def.race.EatsFood)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils")]
        public static class Patch_VomitJob
        {
            [HarmonyPrefix]
            public static bool Prefix(ref JobDriver_Vomit __instance)
            {
                Pawn pawn = __instance.pawn;

                if (pawn.def.HasModExtension<ArtificialPawnProperties>())
                {
                    __instance.ended = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(MentalBreakWorker), "BreakCanOccur")]
        public static class Patch_CanBeResearchedAt_Postfix
        {
            [HarmonyPrefix]
            public static bool Prefix(MentalBreakWorker __instance, bool __result, Pawn pawn)
            {
                if (!pawn.def.race.EatsFood)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }


        // TODO: Terrible patch, needs replaced with a transpiler.
        [HarmonyPatch(typeof(MeditationUtility), "CanMeditateNow")]
        public static class Patch_MeditationUtility_CanMeditateNow
        {
            [HarmonyPrefix]
            public static bool Prefix(MentalBreakWorker __instance, bool __result, Pawn pawn)
            {
                if (!pawn.def.race.EatsFood)
                {
                    __result = CanMeditateNow(pawn);
                    return false;
                }
                return true;
            }

            public static bool CanMeditateNow(Pawn pawn)
            {
                if (pawn.needs.rest != null && pawn.needs.rest.CurCategory >= RestCategory.VeryTired)
                {
                    return false;
                }
                if (pawn.needs.food != null && pawn.needs.food.Starving)
                {
                    return false;
                }
                if (!pawn.Awake())
                {
                    return false;
                }
                if (pawn.health.hediffSet.BleedRateTotal <= 0f)
                {
                    if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                    {
                        Pawn_TimetableTracker timetable = pawn.timetable;
                        if (((timetable != null) ? timetable.CurrentAssignment : null) != TimeAssignmentDefOf.Meditate)
                        {
                            return false;
                        }
                    }
                    if (!HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn))
                    {
                        return true;
                    }
                }
                return false;
            }
        }



        // No special mechanoids in ancient dangers.
        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (kind.race.HasModExtension<ArtificialPawnProperties>()) __result = false;
        }

        // No special mechanoids in crashed ships.
        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (def.race.HasModExtension<ArtificialPawnProperties>()) __result = false;
        }
    }
}
