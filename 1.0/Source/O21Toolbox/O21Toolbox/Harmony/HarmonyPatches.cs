using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using RimWorld.BaseGen;
using Verse;
using Verse.AI;
using Verse.AI.Group;

using Harmony;
using Harmony.ILCopying;

using O21Toolbox;
using O21Toolbox.Alliances;
using O21Toolbox.ApparelExt;
using O21Toolbox.ArtificialPawn;
using O21Toolbox.TurretsPlus;
//using O21Toolbox.Laser;
//using O21Toolbox.ModularWeapon;
using O21Toolbox.Needs;
using O21Toolbox.Networks;
using O21Toolbox.NotQuiteHumanoid;
using O21Toolbox.PawnConverter;
using O21Toolbox.Research;
//using O21Toolbox.Spaceship;
using O21Toolbox.WeaponRestrict;
using O21Toolbox.Utility;

namespace O21Toolbox.Harmony
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

            #region Alliances
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(Faction), "TryMakeInitialRelationsWith", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "TryMakeInitialRelationsWithPostfix", null), null);
            #endregion Alliances

            #region Needs

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

            {
                //Toils_Tend
                Type type = typeof(Toils_Tend);

                //Patch: Toils_Tend.FinalizeTend as Prefix
                O21ToolboxHarmony.Patch(
                    type.GetMethod("FinalizeTend"),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Toils_Tend_FinalizeTend))),
                    null);
            }

            {
                //HealthAIUtility
                Type type = typeof(HealthAIUtility);

                //Patch: HealthAIUtility.FindBestMedicine as Prefix
                O21ToolboxHarmony.Patch(
                    type.GetMethod("FindBestMedicine"),
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_HealthAIUtility_FindBestMedicine))),
                    null);
            }
            #endregion EnergyNeed

            #region Apparel
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new Type[]{typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool)}, null), null, new HarmonyMethod(HarmonyPatches.patchType, "RenderPawnInternalPostfix", null), null);
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "AddHumanlikeOrdersPostfix", null), null);
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(JobGiver_OptimizeApparel), "ApparelScoreGain", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "ApparelScoreGainPostFix", null), null);
            //O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor", null, null), new HarmonyMethod(HarmonyPatches.patchType, "GenerateStartingApparelForPrefix", null), new HarmonyMethod(HarmonyPatches.patchType, "GenerateStartingApparelForPostfix", null), null);
            #endregion Apparel

            #region ModularWeapon
            //O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipmentAiming", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "DrawEquipmentAimingPostfix", null), null);
            #endregion ModularWeapon

            #region NotQuiteHumanoid
            O21ToolboxHarmony.Patch(
                typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(mi =>
                        mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                        mi.GetParameters().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)),
                null, new HarmonyMethod(typeof(HarmonyPatches),
                    nameof(NQHFixerAncient)));
            O21ToolboxHarmony.Patch(
                typeof(CompSpawnerMechanoidsOnDamaged).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First(
                    mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                          mi.GetParameters().Count() == 1 &&
                          mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(
                    typeof(HarmonyPatches),
                    nameof(NQHFixer)));
            #endregion

            #region ThirdPartyImrpovements
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Save Our Ship 2"))
            {
                SaveOurShip2_CompatibilityHook(O21ToolboxHarmony);
            }
            #endregion

            O21ToolboxHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Patches

        #region Alliances
        public static void TryMakeInitialRelationsWithPostfix(Faction __instance, Faction other)
        {
            IEnumerable<AllianceDef> enumerable = from def in DefDatabase<AllianceDef>.AllDefs
                                                where def.memberFactions != null
                                                select def;
            foreach (AllianceDef current in enumerable)
            {
                if (current.memberFactions.Contains(__instance.def.defName))
                {
                    foreach(string faction in current.memberFactions)
                    {
                        if(faction != __instance.def.defName && other.def.defName.Contains(faction))
                        {
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = 100;
                            factionRelation.kind = FactionRelationKind.Ally;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = 100;
                            factionRelation2.kind = FactionRelationKind.Ally;
                        }
                    }

                    current.factionRelations.ForEach(delegate (RelationFaction rf)
                    {
                        if (other.def.defName.Contains(rf.faction))
                        {
                            int relation = rf.relation;
                            FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = relation;
                            factionRelation.kind = kind;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = relation;
                            factionRelation2.kind = kind;
                        }
                    });

                    current.allianceRelations.ForEach(delegate (RelationAlliance ra)
                    {
                        if (ra.alliance.memberFactions.Contains(other.def.defName))
                        {
                            int relation = ra.relation;
                            FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = relation;
                            factionRelation.kind = kind;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = relation;
                            factionRelation2.kind = kind;
                        }
                    });
                }

                current.playerRelations.ForEach(delegate (RelationPlayer rp)
                {
                    PawnKindDef basicMemberKind = __instance.def.basicMemberKind;
                    if (basicMemberKind != null && rp.factionBasicMemberKind.Contains(basicMemberKind.defName) && current.memberFactions.Contains(other.def.defName))
                    {
                        int relation = rp.relation;
                        FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                        FactionRelation factionRelation = other.RelationWith(__instance, false);
                        factionRelation.goodwill = relation;
                        factionRelation.kind = kind;
                        FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                        factionRelation2.goodwill = relation;
                        factionRelation2.kind = kind;
                    }
                });
            }
        }
        #endregion Alliances

        #region ResearchBenchSub
        /// <summary>
        /// Patch for enabling comped research benches to act like they either are another bench, or act like they have a specific facility attached.
        /// </summary>[HarmonyPriority(Priority.High)]
        [HarmonyPatch(typeof(ResearchProjectDef), "PlayerHasAnyAppropriateResearchBench", MethodType.Getter)]
        public static class PlayerHasAnyAppropriateResearchBench_Postfix
        {
            public static void PostFix(ResearchProjectDef __instance, ref bool __result)
            {
                if (!__result)
                {
                    List<Map> maps = Find.Maps;
                    for (int i = 0; i < maps.Count; i++)
                    {
                        List<Building> allBuildingsColonist = maps[i].listerBuildings.allBuildingsColonist;
                        for (int j = 0; j < allBuildingsColonist.Count; j++)
                        {
                            DefModExt_ResearchBenchSubstitutes comp = allBuildingsColonist[j].def.TryGetModExtension<DefModExt_ResearchBenchSubstitutes>();
                            if (comp != null)
                            {
                                if (comp.actLikeResearchBench.Contains(__instance.requiredResearchBuilding))
                                {
                                    __result = true;
                                }

                                if (!__instance.requiredResearchFacilities.NullOrEmpty<ThingDef>())
                                {
                                    bool hasFacilities = true;
                                    foreach(ThingDef facility in __instance.requiredResearchFacilities)
                                    {
                                        if (!comp.actLikeResearchFacility.Contains(facility))
                                        {
                                            hasFacilities = false;
                                        }
                                    }
                                    __result = hasFacilities;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion ResearchBenchSub

        #region NeedsPatches

        public static bool Patch_HealthAIUtility_FindBestMedicine(ref Thing __result, Pawn healer, Pawn patient)
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

        public static bool Patch_Toils_Tend_FinalizeTend(ref Toil __result, Pawn patient)
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
        public static bool Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood(
            ref List<Pawn> pawns, List<ThingDefCount> extraFood, int tile, IgnorePawnsInventoryMode ignoreInventory, Faction faction,
            WorldPath path, float nextTileCostLeft, int caravanTicksPerMove, bool assumeCaravanMoving)
        {
            List<Pawn> modifiedPawnsList = new List<Pawn>(pawns);
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<ArtificialPawnProperties>());
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<DefModExt_SolarNeed>());

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
            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_InspirationHandlerTick(ref InspirationHandler __instance)
        {
            if (__instance.pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_AppendThoughts_ForHumanlike(ref Pawn victim)
        {
            if (victim.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.colonyCaresIfDead)
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_InteractionsTrackerTick(ref Pawn_InteractionsTracker __instance)
        {
            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }

        public static bool CompatPatch_CanInteractNowWith(ref Pawn_InteractionsTracker __instance, ref bool __result, ref Pawn recipient)
        {
            //Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if (recipient.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_SocialFightChance(ref Pawn_InteractionsTracker __instance, ref float __result, ref InteractionDef interaction, ref Pawn initiator)
        {
            Pawn pawn = Pawn_InteractionsTracker_GetPawn(__instance);

            if ((pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize) || (initiator.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties propertiesTwo && !propertiesTwo.canSocialize))
            {
                __result = 0f;
                return false;
            }

            return true;
        }

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

        public static bool CompatPatch_GetJoyTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_EatInPartyAreaTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = null;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_ShouldGuestKeepAttendingGathering(ref bool __result, ref Pawn p)
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

        public static bool CompatPatch_TimetablePreventsLayDown(ref bool __result, ref Pawn pawn)
        {
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = pawn.timetable != null && !pawn.timetable.CurrentAssignment.allowRest;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_CalculatePain(ref HediffSet __instance, ref float __result)
        {
            if (__instance.pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                __result = 0f;
                return false;
            }

            return true;
        }

        public static bool CompatPatch_ShouldBeDeadFromRequiredCapacity(ref Pawn_HealthTracker __instance, ref PawnCapacityDef __result)
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
            if (pawn.def.GetModExtension<ArtificialPawnProperties>() is ArtificialPawnProperties properties && !properties.canSocialize)
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
            if(NeedsDefOf.O21Solar != null)
            {
                if(nd == NeedsDefOf.O21Solar)
                {
                    if (pawn.def.HasModExtension<DefModExt_SolarNeed>())
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
            if (!pawn.def.HasModExtension<DefModExt_SolarNeed>())
            {
                if(nd == NeedsDefOf.O21Solar)
                {
                    __result = false;
                }
            }
        }

        public static void Patch_Need_Food_Starving_Get(ref Need_Food __instance, ref bool __result)
        {
            Pawn pawn = Need_Food_Starving_GetPawn(__instance);

            if (pawn != null && pawn.IsArtificialPawn())
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

        #region ApparelPatches

        //public static void RenderPawnInternalPostfix(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump)
        //{
        //    if (!__instance.graphics.pawn.RaceProps.Animal)
        //    {
        //        List<ApparelGraphicRecord> offsetApparelList = new List<ApparelGraphicRecord>();
        //        // Get all apparel with the defModExt.
        //        foreach(Apparel ap in __instance.graphics.pawn.apparel.WornApparel)
        //        {
        //            ApparelGraphicRecord item;
        //            if (ap.def.HasModExtension<DefModExt_HeadwearOffset>())
        //            {
        //                DefModExt_HeadwearOffset modExt = ap.def.GetModExtension<DefModExt_HeadwearOffset>();
        //                if (TryGetGraphicApparelSpecial(ap, __instance.graphics.pawn.story.bodyType, modExt, out item))
        //                {
        //                    offsetApparelList.Add(item);
        //                }
        //            }
        //        }

        //        // Render if any Apparel in the list and NOT an animal.
        //        if (offsetApparelList.Count >= 1)
        //        {
        //            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
        //            for (int i = 0; i < offsetApparelList.Count; i++)
        //            {
        //                DefModExt_HeadwearOffset modExt = offsetApparelList[i].sourceApparel.def.GetModExtension<DefModExt_HeadwearOffset>();
        //                Vector3 baseOffset = quaternion * modExt.offset;
        //                Mesh mesh = __instance.graphics.HairMeshSet.MeshAt(headFacing);
        //                Vector3 loc2 = rootLoc + baseOffset;
        //                loc2.y += 0.03125f;

        //                if (modExt.bodyDependant)
        //                {

        //                }
        //                else
        //                {
        //                    if (!offsetApparelList[i].sourceApparel.def.apparel.hatRenderedFrontOfFace)
        //                    {
        //                        Material material2 = offsetApparelList[i].graphic.MatAt(bodyFacing, null);
        //                        material2 = __instance.graphics.flasher.GetDamagedMat(material2);
        //                        GenDraw.DrawMeshNowOrLater(mesh, loc2, quaternion, material2, portrait);
        //                    }
        //                    else
        //                    {
        //                        Material material3 = offsetApparelList[i].graphic.MatAt(bodyFacing, null);
        //                        material3 = __instance.graphics.flasher.GetDamagedMat(material3);
        //                        Vector3 loc3 = rootLoc + baseOffset;
        //                        loc3.y += ((!(bodyFacing == Rot4.North)) ? 0.03515625f : 0.00390625f);
        //                        GenDraw.DrawMeshNowOrLater(mesh, loc3, quaternion, material3, portrait);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private static bool TryGetGraphicApparelSpecial(Apparel apparel, BodyTypeDef bodyType, DefModExt_HeadwearOffset modExt, out ApparelGraphicRecord rec)
        {
            if (bodyType == null)
            {
                Log.Error("Getting apparel graphic with undefined body type.", false);
                bodyType = BodyTypeDefOf.Male;
            }
            if (modExt.wornGraphicPath.NullOrEmpty())
            {
                rec = new ApparelGraphicRecord(null, null);
                return false;
            }
            string path;
            if (!modExt.bodyDependant)
            {
                path = modExt.wornGraphicPath;
            }
            else
            {
                path = modExt.wornGraphicPath + "_" + bodyType.defName;
            }
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
            rec = new ApparelGraphicRecord(graphic, apparel);
            return true;
        }

        public static Pawn PawnRenderer_GetPawn_GetPawn(PawnRenderer instance)
        {
            return (Pawn)int_PawnRenderer_GetPawn.GetValue(instance);
        }

        /**public static void GenerateStartingApparelForPostfix()
        {
            Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs").GetValue<List<ThingStuffPair>>().AddRange(HarmonyPatches.apparelList);
        }
        
        public static void GenerateStartingApparelForPrefix(Pawn pawn)
        {
            Traverse traverse = Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs");
            HarmonyPatches.apparelList = new HashSet<ThingStuffPair>();
            foreach (ThingStuffPair thingStuffPair in GenList.ListFullCopy<ThingStuffPair>(traverse.GetValue<List<ThingStuffPair>>()))
            {
                if (!RaceRestrictionSettings.CanWear(thingStuffPair.thing, pawn.def))
                {
                    HarmonyPatches.apparelList.Add(thingStuffPair);
                }
                if (!ApparelRestrict.RestrictionCheck.CanWear(thingStuffPair.thing, pawn.story.bodyType))
                {

                }
            }
            traverse.GetValue<List<ThingStuffPair>>().RemoveAll((ThingStuffPair tsp) => HarmonyPatches.apparelList.Contains(tsp));
        }**/

        public static void ApparelScoreGainPostFix(Pawn pawn, Apparel ap, ref float __result)
        {
            if (__result < 0f)
            {
                return;
            }
            if (!pawn.AnimalOrWildMan())
            {
                if (!ApparelExt.RestrictionCheck.CanWear(ap.def, pawn))
                {
                    __result -= 50f;
                }
            }
        }

        public static void AddHumanlikeOrdersPostfix(ref List<FloatMenuOption> opts, Pawn pawn, Vector3 clickPos)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);
            if (pawn.equipment != null)
            {
                ThingWithComps equipment = (ThingWithComps)c.GetThingList(pawn.Map).FirstOrDefault((Thing t) => t.TryGetComp<CompEquippable>() != null && t.def.IsWeapon);
                if (equipment != null)
                {
                    List<FloatMenuOption> list = (from fmo in opts
                                                  where !fmo.Disabled && fmo.Label.Contains("Equip".Translate(equipment.LabelShort))
                                                  select fmo).ToList<FloatMenuOption>();
                    if(!list.NullOrEmpty<FloatMenuOption>() && !WeaponRestrict.RestrictionCheck.CanEquip(equipment.def, pawn))
                    {
                        foreach (FloatMenuOption item2 in list)
                        {
                            int index2 = opts.IndexOf(item2);
                            opts.Remove(item2);
                            opts.Insert(index2, new FloatMenuOption("CannotEquip".Translate(equipment.LabelShortCap) + " (missing required apparel)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                    }
                }
            }
            Apparel apparel = pawn.Map.thingGrid.ThingAt<Apparel>(c);
            if (apparel != null)
            {
                List<FloatMenuOption> list2 = (from fmo in opts
                                               where !fmo.Disabled && fmo.Label.Contains("ForceWear".Translate(apparel.LabelShort, apparel)) && !fmo.Label.Contains("CannotWear".Translate(apparel.LabelShort, apparel))
                                               select fmo).ToList<FloatMenuOption>();
                if (!list2.NullOrEmpty<FloatMenuOption>() && !ApparelExt.RestrictionCheck.CanWear(apparel.def, pawn))
                {
                    foreach (FloatMenuOption item3 in list2)
                    {
                        int index3 = opts.IndexOf(item3);
                        opts.Remove(item3);
                        opts.Insert(index3, new FloatMenuOption("CannotWear".Translate(apparel.LabelShort, apparel) + " (" + pawn.story.bodyType.defName.ToString() + " body can't wear this)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                }
            }
        }
        #endregion ApparelPatches

        #region ModularWeaponPatches
        /** public static void DrawEquipmentAimingPostfix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            float num = aimAngle - 90f;
            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            num %= 360f;
            Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
            Material matSingle;
            if (eq.def.HasModExtension<DefModExtension_ModularWeapon>())
            {
                matSingle = eq.def.GetModExtension<DefModExtension_ModularWeapon>().GetCurrentTexture.MatSingle;
            }
            else if (graphic_StackCount != null)
            {
                matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
            }
            else
            {
                matSingle = eq.Graphic.MatSingle;
            }
            Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
        }**/
        #endregion ModularWeaponPatches

        #region NotQuiteHumanoid


        public static void NQHFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (typeof(NQH_Pawn).IsAssignableFrom(kind.race.thingClass)) __result = false;
        }

        public static void NQHFixer(ref bool __result, PawnKindDef def)
        {
            if (typeof(NQH_Pawn).IsAssignableFrom(def.race.thingClass)) __result = false;
        }

        [HarmonyPatch(typeof(TransferableUtility), "CanStack")]
        public static class TransferableUtility_CanStack_Patch
        {
            static bool Prefix(Thing thing, ref bool __result)
            {
                if (thing.def.category == ThingCategory.Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    if (pawn is NQH_Pawn)
                    {
                        __result = false;
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("ShouldSendNotificationAbout")]
        public static class ShouldSendNotificationPatch
        {
            public static bool Prefix(Pawn p)
            {
                return !(p is NQH_Pawn);
            }
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch("IsColonistPlayerControlled", MethodType.Getter)]
        public static class IsColonistPatch
        {
            public static void Postfix(Pawn __instance, ref bool __result)
            {
                if(__instance is NQH_Pawn)
                {
                    __result = __instance.Spawned && (__instance.Faction != null && __instance.Faction.IsPlayer) && __instance.MentalStateDef == null && __instance.HostFaction == null;
                }
            }
        }

        #endregion NotQuiteHumanoid

        #region ThirdPartyImprovements
        public static void SaveOurShip2_CompatibilityHook(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(SaveOurShip2.ShipInteriorMod2), "hasSpaceSuit"), null, new HarmonyMethod(typeof(HarmonyPatches), "SOS2CompatibilityHook_hasSpaceSuit_Postfix"));
        }
        public static void SOS2CompatibilityHook_hasSpaceSuit_Postfix(Pawn thePawn, ref bool __result)
        {
            if (thePawn != null && __result == false)
            {
                if (thePawn.def.HasModExtension<DefModExt_SpaceCapable>())
                {
                    __result = true;
                }
                else if(thePawn.apparel != null)
                {
                    bool hasHelmet = false;
                    bool hasSuit = false;
                    foreach (Apparel ap in thePawn.apparel.WornApparel)
                    {
                        if (ap.def.HasModExtension<DefModExt_SpaceApparel>())
                        {
                            DefModExt_SpaceApparel ext = ap.def.GetModExtension<DefModExt_SpaceApparel>();
                            if (ext.equipmentType == spaceEquipmentType.full)
                            {
                                hasHelmet = true;
                                hasSuit = true;
                            }
                            else if (ext.equipmentType == spaceEquipmentType.helmet)
                            {
                                hasHelmet = true;
                            }
                            else if (ext.equipmentType == spaceEquipmentType.suit)
                            {
                                hasSuit = true;
                            }
                        }
                    }

                    __result = hasHelmet && hasSuit;
                }
            }
        }
        #endregion
    }
}