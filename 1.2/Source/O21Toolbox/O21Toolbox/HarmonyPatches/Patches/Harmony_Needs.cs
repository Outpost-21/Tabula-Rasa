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
