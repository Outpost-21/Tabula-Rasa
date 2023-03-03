using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace HRF
{
	[HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
	public static class MakeDowned_Patch
	{
		private static void Postfix(Pawn ___pawn, DamageInfo? dinfo, Hediff hediff)
		{
			foreach (var h in ___pawn.health.hediffSet.hediffs)
			{
				if (h is HediffResource hr && hr.CurStage is HediffStageResource hediffStageResource && hediffStageResource.effectWhenDowned != null)
				{
					if (!HRFManager.Instance.pawnDownedStates.TryGetValue(___pawn, out var downedStateData))
					{
						HRFManager.Instance.pawnDownedStates[___pawn] = downedStateData = new DownedStateData
						{
							lastDownedEffectTicks = new Dictionary<HediffResourceDef, int>()
						};
					}
					if (hediffStageResource.effectWhenDowned.ticksBetweenActivations > 0 &&
						(!downedStateData.lastDownedEffectTicks.TryGetValue(hr.def, out var value) || Find.TickManager.TicksGame >= value + hediffStageResource.effectWhenDowned.ticksBetweenActivations))
					{
						downedStateData.lastDownedEffectTicks[hr.def] = Find.TickManager.TicksGame;
						var hediffToApply = hediffStageResource.effectWhenDowned.hediff != null ? hediffStageResource.effectWhenDowned.hediff : hr.def;
						if (hediffToApply is HediffResourceDef resourceDef)
						{
							Utils.AdjustResourceAmount(___pawn, resourceDef, hediffStageResource.effectWhenDowned.apply, true, null, null);
						}
						else
						{
							HealthUtility.AdjustSeverity(___pawn, hediffToApply, hediffStageResource.effectWhenDowned.apply);
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned")]
	public static class Patch_ShouldBeDowned
	{
		private static bool Prefix(Pawn ___pawn)
		{
			if (___pawn?.health?.hediffSet?.hediffs != null)
			{
				foreach (var hediff in ___pawn.health.hediffSet.hediffs)
				{
					if (hediff is HediffResource hediffResource && hediff.CurStage is HediffStageResource hediffStageResource && hediffStageResource.preventDowning)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(HediffSet), "GetPartHealth")]
	public static class Patch_GetPartHealth
	{
		private static void Postfix(ref float __result, HediffSet __instance, BodyPartRecord part)
		{
			if (__result <= 0 && __instance.hediffs != null)
			{
				foreach (var hediff in __instance.hediffs)
				{
					if (hediff is HediffResource hediffResource && hediff.CurStage is HediffStageResource hediffStageResource && hediffStageResource.preventDeath)
					{
						__result = 1f;
						var hediffInjury = __instance.hediffs.OfType<Hediff_Injury>().FirstOrDefault(x => x.part == part);
						if (hediffInjury != null)
                        {
							hediffInjury.Severity -= 1;
                        }
					}
				}
			}
		}
	}
}
