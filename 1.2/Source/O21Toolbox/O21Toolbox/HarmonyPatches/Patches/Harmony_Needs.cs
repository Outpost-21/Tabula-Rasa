using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
        public static FieldInfo int_Pawn_NeedsTracker_GetPawn;
        public static FieldInfo int_PawnRenderer_GetPawn;
        public static FieldInfo int_Need_Food_Starving_GetPawn;
        public static FieldInfo int_ConditionalPercentageNeed_need;
        public static FieldInfo int_Pawn_HealthTracker_GetPawn;
        public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

        public static NeedDef Need_Bladder;
        public static NeedDef Need_Hygiene;

        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {
            //Try get needs.
            Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");
        }

            //Patch, Method: Pawn_NeedsTracker
            //{
            //    Type type = typeof(Pawn_NeedsTracker);

            //    //Get private variable 'pawn' from 'Pawn_NeedsTracker'.
            //    int_Pawn_NeedsTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            //    //Patch: Pawn_NeedsTracker.ShouldHaveNeed as Postfix
            //    O21ToolboxHarmony.Patch(
            //        type.GetMethod("ShouldHaveNeed", BindingFlags.NonPublic | BindingFlags.Instance),
            //        null,
            //        new HarmonyMethod(typeof(HarmonyPatches), "Patch_Pawn_NeedsTracker_ShouldHaveNeed"));
            //}

            //Patch, Property: Need_Food.Starving
            //{
            //    Type type = typeof(Need_Food);

            //    //Get protected variable 'pawn' from 'PawnRenderer'.
            //    int_Need_Food_Starving_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            //    //Get, get method.
            //    MethodInfo getMethod = AccessTools.Property(type, "Starving")?.GetGetMethod();

            //    //Patch: Pawn_NeedsTracker.ShouldHaveNeed as Postfix
            //    O21ToolboxHarmony.Patch(getMethod, null, new HarmonyMethod(typeof(HarmonyPatches), "Patch_Need_Food_Starving_Get"));
            //}

            //Patch, Method: ThinkNode_ConditionalNeedPercentageAbove
            //{
            //    Type type = typeof(ThinkNode_ConditionalNeedPercentageAbove);

            //    int_ConditionalPercentageNeed_need = type.GetField("need", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            //    //Patch: ThinkNode_ConditionalNeedPercentageAbove.Satisfied as Prefix
            //    O21ToolboxHarmony.Patch(
            //        type.GetMethod("Satisfied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance),
            //        new HarmonyMethod(typeof(HarmonyPatches), "Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied"),
            //        null);
            //}

            //{
            //    //DaysWorthOfFoodCalculator
            //    Type type = typeof(DaysWorthOfFoodCalculator);

            //    //Patch: DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood as Prefix
            //    Type[] types = new Type[] {
            //            typeof(List<Pawn>), typeof(List<ThingDefCount>), typeof(int), typeof(IgnorePawnsInventoryMode),
            //            typeof(Faction), typeof(WorldPath), typeof(float), typeof(int), typeof(bool)};
            //    O21ToolboxHarmony.Patch(
            //        type.GetMethod("ApproxDaysWorthOfFood", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static, Type.DefaultBinder, types, null),
            //        new HarmonyMethod(typeof(HarmonyPatches), "Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood"),
            //        null);
            //}

            //Droid
            //Compatibility Patches
            //{
            //    Type type = typeof(FoodUtility);

            //    O21ToolboxHarmony.Patch(type.GetMethod("WillIngestStackCountOf"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_WillIngestStackCountOf"), null);
            //}

        //    {
        //        Type type = typeof(RecordWorker_TimeInBedForMedicalReasons);

        //        O21ToolboxHarmony.Patch(type.GetMethod("ShouldMeasureTimeNow"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_ShouldMeasureTimeNow"), null);
        //    }

        //    {
        //        Type type = typeof(InteractionUtility);

        //        O21ToolboxHarmony.Patch(type.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_CanInitiateInteraction"), null);
        //    }

        //    {
        //        Type type = typeof(Pawn_HealthTracker);

        //        int_Pawn_HealthTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

        //        O21ToolboxHarmony.Patch(type.GetMethod("ShouldBeDeadFromRequiredCapacity"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_ShouldBeDeadFromRequiredCapacity"), null);
        //    }

        //    {
        //        Type type = typeof(HediffSet);

        //        O21ToolboxHarmony.Patch(type.GetMethod("CalculatePain", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_CalculatePain"), null);
        //    }

        //    {
        //        Type type = typeof(RestUtility);

        //        O21ToolboxHarmony.Patch(type.GetMethod("TimetablePreventsLayDown"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_TimetablePreventsLayDown"), null);
        //    }

        //    {
        //        Type type = typeof(GatheringsUtility);

        //        O21ToolboxHarmony.Patch(type.GetMethod("ShouldGuestKeepAttendingGathering"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_ShouldGuestKeepAttendingGathering"), null);
        //    }

        //    {
        //        Type type = typeof(JobGiver_EatInGatheringArea);

        //        O21ToolboxHarmony.Patch(type.GetMethod("TryGiveJob", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_EatInPartyAreaTryGiveJob"), null);
        //    }

        //    {
        //        Type type = typeof(JobGiver_GetJoy);

        //        O21ToolboxHarmony.Patch(type.GetMethod("TryGiveJob", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_GetJoyTryGiveJob"), null);
        //    }

        //    {
        //        Type type = typeof(Pawn_InteractionsTracker);

        //        int_Pawn_InteractionsTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

        //        O21ToolboxHarmony.Patch(type.GetMethod("SocialFightChance"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_SocialFightChance"), null);
        //        O21ToolboxHarmony.Patch(type.GetMethod("InteractionsTrackerTick"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_InteractionsTrackerTick"), null);
        //        O21ToolboxHarmony.Patch(type.GetMethod("CanInteractNowWith"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_CanInteractNowWith"), null);
        //    }

        //    {
        //        Type type = typeof(InteractionUtility);

        //        O21ToolboxHarmony.Patch(type.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_CanDoInteraction"), null);
        //        O21ToolboxHarmony.Patch(type.GetMethod("CanReceiveInteraction"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_CanDoInteraction"), null);
        //    }

        //    {
        //        Type type = typeof(PawnDiedOrDownedThoughtsUtility);

        //        O21ToolboxHarmony.Patch(type.GetMethod("AppendThoughts_ForHumanlike", BindingFlags.NonPublic | BindingFlags.Static), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_AppendThoughts_ForHumanlike"), null);
        //    }

        //    {
        //        Type type = typeof(InspirationHandler);

        //        O21ToolboxHarmony.Patch(type.GetMethod("InspirationHandlerTick"), new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_InspirationHandlerTick"), null);
        //    }

        //    {
        //        Type type = typeof(JobDriver_Vomit);

        //        O21ToolboxHarmony.Patch(
        //            type.GetMethod("MakeNewToils", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod),
        //            new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_VomitJob"),
        //            null);
        //    }

        //    {
        //        Type type = typeof(Alert_Boredom);

        //        //For some reason this did not work.
        //        /*harmony.Patch(
        //            AccessTools.Method(type, "BoredPawns"),
        //            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_BoredPawns))),
        //            null);

        //        Log.Message("Patched Alert_Boredom.BoredPawns");*/

        //        //But this did.
        //        O21ToolboxHarmony.Patch(
        //            AccessTools.Method(type, "GetReport"),
        //            new HarmonyMethod(typeof(HarmonyPatches), "CompatPatch_Boredom_GetReport"),
        //            null);

        //        //Log.Message("Patched Alert_Boredom.BoredPawns");
        //    }

        //    {
        //        //Toils_Tend
        //        Type type = typeof(Toils_Tend);

        //        //Patch: Toils_Tend.FinalizeTend as Prefix
        //        O21ToolboxHarmony.Patch(
        //            type.GetMethod("FinalizeTend"),
        //            new HarmonyMethod(typeof(HarmonyPatches), "Patch_Toils_Tend_FinalizeTend"),
        //            null);
        //    }

        //    {
        //        //HealthAIUtility
        //        Type type = typeof(HealthAIUtility);

        //        //Patch: HealthAIUtility.FindBestMedicine as Prefix
        //        O21ToolboxHarmony.Patch(
        //            type.GetMethod("FindBestMedicine"),
        //            new HarmonyMethod(typeof(HarmonyPatches), "Patch_HealthAIUtility_FindBestMedicine"),
        //            null);
        //    }
        //}

        






        /*public static bool CompatPatch_VomitJob(ref JobDriver_Vomit __instance)
        {
            Pawn pawn = __instance.pawn;

            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                //__instance.ended = true;
                return false;
            }

            return true;
        }*/




        

        

        //erdelf: No special mechanoids in ancient dangers.
        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (kind.race.HasModExtension<ArtificialPawnProperties>()) __result = false;
        }

        //erdelf:  No special mechanoids in crashed ships.
        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (def.race.HasModExtension<ArtificialPawnProperties>()) __result = false;
        }

        








        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        //public static Pawn Pawn_NeedsTracker_GetPawn(Pawn_NeedsTracker instance)
        //{
        //    return (Pawn)int_Pawn_NeedsTracker_GetPawn.GetValue(instance);
        //}

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        //public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
        //{
        //    return (Pawn)int_Pawn_HealthTracker_GetPawn.GetValue(instance);
        //}

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the PawnRenderer class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        //public static NeedDef ThinkNode_ConditionalNeedPercentageAbove_GetNeed(ThinkNode_ConditionalNeedPercentageAbove instance)
        //{
        //    return (NeedDef)int_ConditionalPercentageNeed_need.GetValue(instance);
        //}

        /// <summary>
        /// Adds a null check.
        /// </summary>
        //public static bool Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied(ref ThinkNode_ConditionalNeedPercentageAbove __instance, ref bool __result, ref Pawn pawn)
        //{
        //    NeedDef need = ThinkNode_ConditionalNeedPercentageAbove_GetNeed(__instance);
        //    bool haveNeed = pawn.needs.TryGetNeed(need) != null;

        //    if (!haveNeed)
        //    {
        //        __result = false;
        //        return false;
        //    }

        //    return true;
        //}

        /// <summary>
        /// Adds an additional check for our custom needs.
        /// </summary>
        //public static void Patch_Pawn_NeedsTracker_ShouldHaveNeed(Pawn_NeedsTracker __instance, ref bool __result, ref NeedDef nd)
        //{
        //    //Do not bother checking if our need do not exist.
        //    Pawn pawn = Pawn_NeedsTracker_GetPawn(__instance);

        //    if (NeedsDefOf.O21Energy != null)
        //    {
        //        //Is the need our Energy need?
        //        if (nd == NeedsDefOf.O21Energy)
        //        {
        //            if (pawn.def.HasModExtension<EnergyHediffs>())
        //            {
        //                __result = true;
        //            }
        //            else
        //            {
        //                __result = false;
        //            }
        //        }
        //    }
        //    if (NeedsDefOf.O21Solar != null)
        //    {
        //        if (nd == NeedsDefOf.O21Solar)
        //        {
        //            if (pawn.def.HasModExtension<DefModExt_SolarNeed>())
        //            {
        //                __result = true;
        //            }
        //            else
        //            {
        //                __result = false;
        //            }
        //        }
        //    }

        //    if (!O21ToolboxSettings.Instance.EnergyNeedCompatMode)
        //    {
        //        if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest || nd == NeedDefOf.Joy || nd == NeedsDefOf.Beauty || nd == NeedsDefOf.Comfort || nd == NeedsDefOf.RoomSize || nd == NeedsDefOf.Outdoors || (Need_Bladder != null && nd == Need_Bladder) || (Need_Hygiene != null && nd == Need_Hygiene))
        //        {
        //            if (pawn.def.HasModExtension<EnergyHediffs>())
        //            {
        //                __result = false;
        //            }
        //        }
        //    }

        //    if (!pawn.def.HasModExtension<EnergyHediffs>())
        //    {
        //        if (nd == NeedsDefOf.O21Energy)
        //        {
        //            __result = false;
        //        }
        //    }
        //    if (!pawn.def.HasModExtension<DefModExt_SolarNeed>())
        //    {
        //        if (nd == NeedsDefOf.O21Solar)
        //        {
        //            __result = false;
        //        }
        //    }
        //}


        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Need_Food class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        //public static Pawn Need_Food_Starving_GetPawn(Need_Food instance)
        //{
        //    return (Pawn)int_Need_Food_Starving_GetPawn.GetValue(instance);
        //}

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        //public static Pawn Pawn_InteractionsTracker_GetPawn(Pawn_InteractionsTracker instance)
        //{
        //    return (Pawn)int_Pawn_InteractionsTracker_GetPawn.GetValue(instance);
        //}
    }
}
