using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
using Harmony.ILCopying;
using UnityEngine;
using RimWorld;
using Verse;
using System.Reflection;
using O21Toolbox.ResearchBenchSub;
using O21Toolbox.Needs;
using O21Toolbox.ApparelRestrict;
using Verse.AI;
using RimWorld.Planet;
using AlienRace;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
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

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            //Try get needs.
            Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");

            HarmonyInstance O21ToolboxHarmony = HarmonyInstance.Create("com.o21toolbox.rimworld.mod");

            #region EnergyNeed

            //Patch, Method: Pawn_NeedsTracker
            {
                Type type = typeof(Pawn_NeedsTracker);

                //Get private variable 'pawn' from 'Pawn_NeedsTracker'.
                int_Pawn_NeedsTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                //Patch: Pawn_NeedsTracker.ShouldHaveNeed as Postfix
                O21ToolboxHarmony.Patch(
                    type.GetMethod("ShouldHaveNeed", BindingFlags.NonPublic | BindingFlags.Instance),
                    null,
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Pawn_NeedsTracker_ShouldHaveNeed))));
            }

            //Patch, Property: Need_Food.Starving
            {
                Type type = typeof(Need_Food);

                //Get protected variable 'pawn' from 'PawnRenderer'.
                int_Need_Food_Starving_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                //Get, get method.
                MethodInfo getMethod = AccessTools.Property(type, "Starving")?.GetGetMethod();

                //Patch: Pawn_NeedsTracker.ShouldHaveNeed as Postfix
                O21ToolboxHarmony.Patch(getMethod, null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Need_Food_Starving_Get))));
            }

            //Patch, Method: ThinkNode_ConditionalNeedPercentageAbove
            {
                Type type = typeof(ThinkNode_ConditionalNeedPercentageAbove);

                int_ConditionalPercentageNeed_need = type.GetField("need", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                //Patch: ThinkNode_ConditionalNeedPercentageAbove.Satisfied as Prefix
                O21ToolboxHarmony.Patch(
                    type.GetMethod("Satisfied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied))),
                    null);
            }

            {
                //DaysWorthOfFoodCalculator
                Type type = typeof(DaysWorthOfFoodCalculator);

                //Patch: DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood as Prefix
                Type[] types = new Type[] {
                        typeof(List<Pawn>), typeof(List<ThingDefCount>), typeof(int), typeof(IgnorePawnsInventoryMode),
                        typeof(Faction), typeof(WorldPath), typeof(float), typeof(int), typeof(bool)};
                O21ToolboxHarmony.Patch(
                    type.GetMethod("ApproxDaysWorthOfFood", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static, Type.DefaultBinder, types, null),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood))),
                    null);
            }

            //Droid
            //Compatibility Patches
            {
                Type type = typeof(FoodUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("WillIngestStackCountOf"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_WillIngestStackCountOf))), null);
            }

            {
                Type type = typeof(RecordWorker_TimeInBedForMedicalReasons);

                O21ToolboxHarmony.Patch(type.GetMethod("ShouldMeasureTimeNow"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_ShouldMeasureTimeNow))), null);
            }

            {
                Type type = typeof(InteractionUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_CanInitiateInteraction))), null);
            }

            {
                Type type = typeof(Pawn_HealthTracker);

                int_Pawn_HealthTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                O21ToolboxHarmony.Patch(type.GetMethod("ShouldBeDeadFromRequiredCapacity"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_ShouldBeDeadFromRequiredCapacity))), null);
            }

            {
                Type type = typeof(HediffSet);

                O21ToolboxHarmony.Patch(type.GetMethod("CalculatePain", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_CalculatePain))), null);
            }

            {
                Type type = typeof(RestUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("TimetablePreventsLayDown"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_TimetablePreventsLayDown))), null);
            }

            {
                Type type = typeof(GatheringsUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("ShouldGuestKeepAttendingGathering"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_ShouldGuestKeepAttendingGathering))), null);
            }

            {
                Type type = typeof(JobGiver_EatInPartyArea);

                O21ToolboxHarmony.Patch(type.GetMethod("TryGiveJob", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_EatInPartyAreaTryGiveJob))), null);
            }

            {
                Type type = typeof(JobGiver_GetJoy);

                O21ToolboxHarmony.Patch(type.GetMethod("TryGiveJob", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_GetJoyTryGiveJob))), null);
            }

            {
                Type type = typeof(Pawn_InteractionsTracker);

                int_Pawn_InteractionsTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                O21ToolboxHarmony.Patch(type.GetMethod("SocialFightChance"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_SocialFightChance))), null);
                O21ToolboxHarmony.Patch(type.GetMethod("InteractionsTrackerTick"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_InteractionsTrackerTick))), null);
                O21ToolboxHarmony.Patch(type.GetMethod("CanInteractNowWith"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_CanInteractNowWith))), null);
            }

            {
                Type type = typeof(InteractionUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_CanDoInteraction))), null);
                O21ToolboxHarmony.Patch(type.GetMethod("CanReceiveInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_CanDoInteraction))), null);
            }

            {
                Type type = typeof(PawnDiedOrDownedThoughtsUtility);

                O21ToolboxHarmony.Patch(type.GetMethod("AppendThoughts_ForHumanlike", BindingFlags.NonPublic | BindingFlags.Static), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_AppendThoughts_ForHumanlike))), null);
            }

            {
                Type type = typeof(InspirationHandler);

                O21ToolboxHarmony.Patch(type.GetMethod("InspirationHandlerTick"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_InspirationHandlerTick))), null);
            }

            {
                Type type = typeof(JobDriver_Vomit);

                O21ToolboxHarmony.Patch(
                    type.GetMethod("MakeNewToils", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_VomitJob))),
                    null);
            }

            {
                Type type = typeof(Alert_Boredom);

                //For some reason this did not work.
                /*harmony.Patch(
                    AccessTools.Method(type, "BoredPawns"),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_BoredPawns))),
                    null);

                Log.Message("Patched Alert_Boredom.BoredPawns");*/

                //But this did.
                O21ToolboxHarmony.Patch(
                    AccessTools.Method(type, "GetReport"),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(CompatPatch_Boredom_GetReport))),
                    null);

                //Log.Message("Patched Alert_Boredom.BoredPawns");
            }
            #endregion EnergyNeed

            #region ApparelRestrict
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "AddHumanlikeOrdersPostfix", null), null);
            #endregion ApparelRestrict

            O21ToolboxHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Patches

        #region ResearchBenchSub
        /// <summary>
        /// Patch for enabling comped research benches to act like they either are another bench, or act like they have a specific facility attached.
        /// </summary>
        /**
        [HarmonyPatch(typeof(ResearchProjectDef))]
        [HarmonyPatch(nameof(ResearchProjectDef.CanBeResearchedAt))]
        static class ResearchProjectDef_CanBeResearchedAt_Patch
        {
            static bool Prefix(ref bool __result, ref Building_ResearchBench bench, ResearchProjectDef ___instance)
            {
                // Only do anything if the result is false and the bench has the new comp.
                if (bench.TryGetComp<Comp_ResearchBenchSubstitutes>() != null && !__result)
                {
                    // Does the research have a required building?
                    if(___instance.requiredResearchBuilding != null)
                    {
                        if (bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchBench.Contains(___instance.requiredResearchBuilding))
                        {
                            return true;
                        }
                    }
                    // Does the research have a required facility?
                    if(___instance.requiredResearchFacilities != null)
                    {
                        if(bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchFacility != null)
                        {
                            int i;
                            for (i = 0; i < ___instance.requiredResearchFacilities.Count; i++)
                            {
                                if(bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchFacility.Contains(___instance.requiredResearchFacilities[i]))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return __result;
            }
        }
        **/
        #endregion ResearchBenchSub

        #region EnergyNeedPatches
        public static bool Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood(
            ref List<Pawn> pawns, List<ThingDefCount> extraFood, int tile, IgnorePawnsInventoryMode ignoreInventory, Faction faction,
            WorldPath path, float nextTileCostLeft, int caravanTicksPerMove, bool assumeCaravanMoving)
        {
            List<Pawn> modifiedPawnsList = new List<Pawn>(pawns);
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<MechanicalPawnProperties>());

            pawns = modifiedPawnsList;
            return true;
        }


        public static bool CompatPatch_Boredom_GetReport(ref Alert_Boredom __instance, ref AlertReport __result)
        {
            IEnumerable<Pawn> culprits = null;
            CompatPatch_BoredPawns(ref culprits);

            __result = AlertReport.CulpritsAre(culprits);
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

        public static bool CompatPatch_VomitJob(ref JobDriver_Vomit __instance, ref IEnumerable<Toil> __result)
        {
            Pawn pawn = __instance.pawn;

            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
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

        public static bool CompatPatch_CanDoInteraction(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_InspirationHandlerTick(ref InspirationHandler __instance)
        {
            if (__instance.pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_AppendThoughts_ForHumanlike(ref Pawn victim)
        {
            if (victim.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.colonyCaresIfDead)
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_InteractionsTrackerTick(ref Pawn_InteractionsTracker __instance)
        {
            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_CanInteractNowWith(ref Pawn_InteractionsTracker __instance, ref bool __result, ref Pawn recipient)
        {
            //Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (recipient.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_SocialFightChance(ref Pawn_InteractionsTracker __instance, ref float __result, ref InteractionDef interaction, ref Pawn initiator)
        {
            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if ((pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize) || (initiator.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties propertiesTwo && !propertiesTwo.canSocialize))
            {
                __result = 0f;
                return false;
            }

            return true;
        }

        //erdelf: No special mechanoids in ancient dangers.
        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (kind.race.HasModExtension<MechanicalPawnProperties>()) __result = false;
        }

        //erdelf:  No special mechanoids in crashed ships.
        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (def.race.HasModExtension<MechanicalPawnProperties>()) __result = false;
        }

        public static bool CompatPatch_GetJoyTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_EatInPartyAreaTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_ShouldGuestKeepAttendingGathering(ref bool __result, ref Pawn p)
        {
            //Log.Message("Guest p=" + p?.ToString() ?? "null");
            if (p.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
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

        public static bool CompatPatch_TimetablePreventsLayDown(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                __result = pawn.timetable != null && !pawn.timetable.CurrentAssignment.allowRest;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_CalculatePain(ref HediffSet __instance, ref float __result)
        {
            if (__instance.pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                __result = 0f;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_ShouldBeDeadFromRequiredCapacity(ref Pawn_HealthTracker __instance, ref PawnCapacityDef __result)
        {
            Pawn pawn = Pawn_HealthTracker_GetPawn(__instance);

            if (pawn.def.HasModExtension<MechanicalPawnProperties>())
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

        public static bool CompatPatch_WillIngestStackCountOf(int __result, ref Pawn ingester, ref ThingDef def)
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

        public static bool CompatPatch_ShouldMeasureTimeNow(bool __result, ref Pawn pawn)
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

        public static bool CompatPatch_CanInitiateInteraction(bool __result, ref Pawn pawn)
        {
            if (pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static Pawn Pawn_NeedsTracker_GetPawn(Pawn_NeedsTracker instance)
        {
            return (Pawn)int_Pawn_NeedsTracker_GetPawn.GetValue(instance);
        }

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
        {
            return (Pawn)int_Pawn_HealthTracker_GetPawn.GetValue(instance);
        }

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the PawnRenderer class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static NeedDef ThinkNode_ConditionalNeedPercentageAbove_GetNeed(ThinkNode_ConditionalNeedPercentageAbove instance)
        {
            return (NeedDef)int_ConditionalPercentageNeed_need.GetValue(instance);
        }

        /// <summary>
        /// Adds a null check.
        /// </summary>
        public static bool Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied(ref ThinkNode_ConditionalNeedPercentageAbove __instance, ref bool __result, ref Pawn pawn)
        {
            NeedDef need = ThinkNode_ConditionalNeedPercentageAbove_GetNeed(__instance);
            bool haveNeed = pawn.needs.TryGetNeed(need) != null;

            if (!haveNeed)
            {
                __result = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds an additional check for our custom needs.
        /// </summary>
        public static void Patch_Pawn_NeedsTracker_ShouldHaveNeed(ref Pawn_NeedsTracker __instance, ref bool __result, ref NeedDef nd)
        {
            //Do not bother checking if our need do not exist.
            Pawn pawn = Pawn_NeedsTracker_GetPawn(__instance);

            if (NeedsDefOf.O21Energy != null)
            {
                //Is the need our Energy need?
                if (nd == NeedsDefOf.O21Energy)
                {
                    if (pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = true;
                    }
                    else
                    {
                        __result = false;
                    }
                }
            }

            if (!O21ToolboxSettings.Instance.EnergyNeedCompatMode)
            {
                if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest || nd == NeedDefOf.Joy || nd == NeedsDefOf.Beauty || nd == NeedsDefOf.Comfort || nd == NeedsDefOf.RoomSize || nd == NeedsDefOf.Outdoors || (Need_Bladder != null && nd == Need_Bladder) || (Need_Hygiene != null && nd == Need_Hygiene))
                {
                    if (pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = false;
                    }
                }
            }

            if (!pawn.def.HasModExtension<EnergyHediffs>())
            {
                if(nd == NeedsDefOf.O21Energy)
                {
                    __result = false;
                }
            }
        }

        public static void Patch_Need_Food_Starving_Get(ref Need_Food __instance, ref bool __result)
        {
            Pawn pawn = Need_Food_Starving_GetPawn(__instance);

            if (pawn != null && pawn.IsMechanical())
                __result = false;
        }

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Need_Food class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static Pawn Need_Food_Starving_GetPawn(Need_Food instance)
        {
            return (Pawn)int_Need_Food_Starving_GetPawn.GetValue(instance);
        }

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static Pawn Pawn_InteractionsTracker_GetPawn(Pawn_InteractionsTracker instance)
        {
            return (Pawn)int_Pawn_InteractionsTracker_GetPawn.GetValue(instance);
        }
        #endregion NeedEnergyPatches

        #region ApparelRestrictPatches

        /** public static void GenerateStartingApparelForPrefix(Pawn pawn)
        {
            Traverse traverse = Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs");
            HarmonyPatches.apparelList = new HashSet<ThingStuffPair>();
            foreach (ThingStuffPair thingStuffPair in traverse.GetValue<List<ThingStuffPair>>().ListFullCopy<ThingStuffPair>())
            {
                if (!RaceRestrictionSettings.CanWear(thingStuffPair.thing, pawn.def))
                {
                    HarmonyPatches.apparelList.Add(thingStuffPair);
                }
            }
            traverse.GetValue<List<ThingStuffPair>>().RemoveAll((ThingStuffPair tsp) => HarmonyPatches.apparelList.Contains(tsp));
        } **/

        public static void AddHumanlikeOrdersPostfix(ref List<FloatMenuOption> opts, Pawn pawn, Vector3 clickPos)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);
            if (pawn.apparel == null)
            {
                return;
            }
            Apparel apparel = pawn.Map.thingGrid.ThingAt<Apparel>(c);
            if (apparel == null)
            {
                return;
            }
            List<FloatMenuOption> list2 = (from fmo in opts
                                           where !fmo.Disabled && fmo.Label.Contains("ForceWear".Translate(apparel.LabelShort))
                                           select fmo).ToList<FloatMenuOption>();
            if(list2.NullOrEmpty<FloatMenuOption>() || RaceRestrictionSettings.CanWear(apparel.def, pawn.def))
            {
                if (apparel.def.GetCompProperties<CompProperties_BodyRestrict>() == null)
                {
                    return;
                }
                if (apparel.def.GetCompProperties<CompProperties_BodyRestrict>() != null)
                {
                    if (RestrictionCheck.CanWear(apparel, pawn.story.bodyType))
                    {
                        return;
                    }
                }
                foreach (FloatMenuOption item3 in list2)
                {
                    int index3 = opts.IndexOf(item3);
                    opts.Remove(item3);
                    opts.Insert(index3, new FloatMenuOption("CannotWear".Translate(apparel.LabelShort) + " (" + pawn.story.bodyType.LabelCap + " can't wear this)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                return;
            }
            foreach (FloatMenuOption item3 in list2)
            {
                int index3 = opts.IndexOf(item3);
                opts.Remove(item3);
                opts.Insert(index3, new FloatMenuOption("CannotWear".Translate(apparel.LabelShort) + " (" + pawn.def.LabelCap + " can't wear this)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
        }
        #endregion ApparelRestrictPatches
    }
}