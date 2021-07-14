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
using O21Toolbox.SimpleNeeds;

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
        public static string foodNeedLabel;
        public static string foodNeedDescription;

        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {
            //Try get needs.
            Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");
        }

        [HarmonyPatch(typeof(NeedsCardUtility), "UpdateDisplayNeeds")]
        public static class Patch_NeedsCardUtility_UpdateDisplayNeeds
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn)
            {
                if(foodNeedLabel.NullOrEmpty())
                {
                    foodNeedLabel = NeedDefOf.Food.cachedLabelCap;
                }

                if (foodNeedDescription.NullOrEmpty())
                {
                    foodNeedDescription = NeedDefOf.Food.description;
                }

                if (pawn != null && pawn.def.HasModExtension<DefModExt_FoodNeedAdjuster>())
                {
                    DefModExt_FoodNeedAdjuster modExt = pawn.def.GetModExtension<DefModExt_FoodNeedAdjuster>();
                    if (modExt.newLabel != null)
                    {
                        NeedDefOf.Food.cachedLabelCap = modExt.newLabel;
                    }

                    if (modExt.newDescription != null)
                    {
                        NeedDefOf.Food.description = modExt.newDescription;
                    }
                }
                else
                {
                    NeedDefOf.Food.cachedLabelCap = foodNeedLabel;
                    NeedDefOf.Food.description = foodNeedDescription;
                }
            }
        }

        [HarmonyPatch(typeof(Need_Food), "NeedInterval")]
        public static class Patch_Need_Food_NeedInterval
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Need_Food __instance)
            {
                if (__instance.pawn.def.HasModExtension<DefModExt_FoodNeedAdjuster>())
                {
                    DefModExt_FoodNeedAdjuster modExt = __instance.pawn.def.GetModExtension<DefModExt_FoodNeedAdjuster>();

                    if (modExt.disableFoodNeed)
                    {
                        __instance.CurLevel = __instance.MaxLevel;
                        if (modExt.malnutritionReplacer != null)
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, -__instance.MalnutritionSeverityPerInterval);
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, -__instance.MalnutritionSeverityPerInterval);
                        }
                        return false;
                    }

                    if (!__instance.IsFrozen)
                    {
                        __instance.CurLevel -= __instance.FoodFallPerTick * 150f;
                    }
                    if (!__instance.Starving)
                    {
                        __instance.lastNonStarvingTick = Find.TickManager.TicksGame;
                    }

                    if(modExt.malnutritionReplacer != null)
                    {
                        if (!__instance.IsFrozen)
                        {
                            if (__instance.Starving)
                            {
                                HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, __instance.MalnutritionSeverityPerInterval);
                            }
                            else
                            {
                                HealthUtility.AdjustSeverity(__instance.pawn, modExt.malnutritionReplacer, -__instance.MalnutritionSeverityPerInterval);
                            }
                        }
                    }
                    else
                    {
                        if (!__instance.IsFrozen)
                        {
                            if (__instance.Starving)
                            {
                                HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, __instance.MalnutritionSeverityPerInterval);
                            }
                            else
                            {
                                HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.Malnutrition, -__instance.MalnutritionSeverityPerInterval);
                            }
                        }
                    }

                    return false;
                }

                return true;
            }
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

        [HarmonyPatch(typeof(Thing), "Ingested")]
        public static class Patch_Thing_Ingested
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Thing __instance, Pawn ingester, float nutritionWanted)
            {
                if (__instance.def.HasModExtension<DefModExt_OutputFromEdible>())
                {
                    Thing thing = ThingMaker.MakeThing(__instance.def.GetModExtension<DefModExt_OutputFromEdible>().outputThing);
                    if (!GenPlace.TryPlaceThing(thing, ingester.Position, ingester.Map, ThingPlaceMode.Near))
                    {
                        Log.Error(string.Concat(new object[]
                        {
                                            ingester,
                                            " could not drop recipe product ",
                                            thing,
                                            " near ",
                                            ingester.Position
                        }));
                    }
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

                if (pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
                {
                    __instance.ended = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(MentalBreakWorker), "BreakCanOccur")]
        public static class Patch_MentalBreakWorker_BreakCanOccur
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

        // No special mechanoids in ancient dangers.
        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (kind.race.HasModExtension<DefModExt_ArtificialPawn>()) __result = false;
        }

        // No special mechanoids in crashed ships.
        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (def.race.HasModExtension<DefModExt_ArtificialPawn>()) __result = false;
        }
    }
}
