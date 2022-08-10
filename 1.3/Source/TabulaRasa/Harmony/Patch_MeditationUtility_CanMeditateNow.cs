using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(MeditationUtility), "CanMeditateNow")]
    public static class Patch_MeditationUtility_CanMeditateNow
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, bool __result)
        {
            // Fixes a vanilla bug. Not the best way to do it but I'm sick of dancing around Ludeons failures.
            if (pawn.needs.food == null)
			{
				if (pawn.needs.rest != null && (int)pawn.needs.rest.CurCategory >= 2)
				{
					__result = false;
					return false;
				}
				if (!pawn.Awake())
				{
					__result = false;
					return false;
				}
				if (pawn.health.hediffSet.BleedRateTotal > 0f || (HealthAIUtility.ShouldSeekMedicalRest(pawn) && pawn.timetable?.CurrentAssignment != TimeAssignmentDefOf.Meditate) || HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn))
				{
					__result = false;
					return false;
				}
				__result = true;
				return false;
			}
            return true;
        }
    }
}
