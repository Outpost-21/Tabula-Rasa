using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt")]
	public static class Patch_RenderPawnAt
	{
		public static void Postfix(Pawn ___pawn)
		{
			foreach (var hediff in ___pawn.health.hediffSet.hediffs)
			{
				if (hediff is IDrawable drawable)
				{
                    drawable.Draw();
                }
            }
		}
	}
	[HarmonyPatch(typeof(Pawn), "SetFaction")]
	public static class Patch_SetFaction
	{
		private static void Postfix(Pawn __instance)
		{
			if (__instance?.Faction == Faction.OfPlayerSilentFail && __instance.RaceProps.Humanlike)
			{
				HRFManager.Instance.RegisterAndRecheckForPolicies(__instance);
			}
		}
	}
	public class HediffGenerationData
	{
		public HediffDef hediff;

		public float initialSeverity;

		public float chance = 1f;
	}

	public class PawnKindExtension : DefModExtension
	{
		public List<HediffGenerationData> hediffsOnGeneration;
	}

	[HarmonyPatch(typeof(PawnGenerator))]
	[HarmonyPatch("GeneratePawn", new Type[]
	{
		typeof(PawnGenerationRequest)
	})]
	[StaticConstructorOnStartup]
	public static class PawnGenerator_GeneratePawn
	{
		private static void Postfix(ref Pawn __result)
		{
			if (__result?.Faction == Faction.OfPlayerSilentFail && __result.RaceProps.Humanlike)
			{
				HRFManager.Instance.RegisterAndRecheckForPolicies(__result);
			}
			if (__result.skills?.skills != null)
			{
				foreach (var skill in __result.skills.skills)
				{
					Utils.TryAssignNewSkillRelatedHediffs(skill, __result);
				}
			}

			var extension = __result.kindDef.GetModExtension<PawnKindExtension>();
			if (extension != null)
			{
				if (extension.hediffsOnGeneration != null)
                {
					foreach (var hediffData in extension.hediffsOnGeneration)
                    {
						if (Rand.Chance(hediffData.chance))
                        {
							if (hediffData.hediff is HediffResourceDef resourceDef)
							{
								Utils.AdjustResourceAmount(__result, resourceDef, hediffData.initialSeverity, true, null, null);
							}
							else
							{
								HealthUtility.AdjustSeverity(__result, hediffData.hediff, hediffData.initialSeverity);
							}
						}
                    }
                }
			}
		}
	}

	[HarmonyPatch(typeof(SkillRecord), "Learn")]
	public static class Patch_Learn
	{
		private static void Postfix(SkillRecord __instance, Pawn ___pawn, float xp)
		{
			Utils.TryAssignNewSkillRelatedHediffs(__instance, ___pawn);
			if (___pawn.HasPawnClassComp(out var comp) && comp.HasClass(out var classTrait) && classTrait.xpPerSkillGain > 0 && xp > 0)
			{
				comp.GainXP(xp * classTrait.xpPerSkillGain);

            }
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Patch_Kill
	{
		private static bool Prefix(Pawn __instance)
		{
			if (__instance?.health?.hediffSet?.hediffs != null)
            {
				foreach (var hediff in __instance.health.hediffSet.hediffs)
                {
					if (hediff is HediffResource hediffResource && hediff.CurStage is HediffStageResource hediffStageResource && hediffStageResource.preventDeath)
                    {
						return false;
                    }
                }
            }
			return true;
		}
        private static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
            if (__instance.Dead)
            {
                if (dinfo.HasValue && dinfo.Value.Instigator is Pawn killer)
                {
                    var comp = killer.GetComp<CompPawnClass>();
                    if (comp != null && comp.HasClass(out var traitDef))
                    {
                        if (__instance.RaceProps.Humanlike && traitDef.xpPerHumanlikeValueWhenKilling > 0)
                        {
                            comp.GainXP(__instance.MarketValue * traitDef.xpPerHumanlikeValueWhenKilling);
                        }
                        else if (traitDef.xpPerNonHumanValueWhenKilling > 0)
                        {
                            comp.GainXP(__instance.MarketValue * traitDef.xpPerNonHumanValueWhenKilling);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TraitSet), "GainTrait")]
    public static class TraitSet_GainTrait_Patch
    {
        private static void Postfix(Pawn ___pawn, Trait trait)
        {
			if (trait.def is ClassTraitDef traitDef)
			{
				var comp = ___pawn.GetComp<CompPawnClass>();
				comp.Init(traitDef);
				if (traitDef.randomLevelAtPawnGeneration != null && PawnGenerator.IsBeingGenerated(___pawn))
				{
					var point = traitDef.randomLevelAtPawnGeneration.RandomElementByWeight(x => x.y);
					var randomLevel = (int)point.x;
					if (randomLevel > comp.level)
					{
						comp.UpgradeTo(randomLevel);
                    }
                    while (true)
					{
                        var abilities = ___pawn.GetAbilities();
						bool learned = false;
                        foreach (var entry in abilities)
                        {
                            var abilityData = comp.GetAbilityDataFrom(entry.abilityDef);
                            var nextAbility = entry.GetLearnableAbility(abilityData);
                            var ability = entry.learned ? comp.GetLearnedAbility(entry.abilityDef) : null;
                            bool learnedAbility = ability != null;
                            bool fullyUnlocked = nextAbility is null && ability != null;
                            var canLearnNextTier = fullyUnlocked ? false : comp.CanUnlockAbility(nextAbility);
                            if (canLearnNextTier)
                            {
                                comp.LearnAbility(nextAbility);
								learned = true;
                                break;
                            }
                            else continue;
                        }
						if (!learned)
						{
							break;
						}
                    }

                }
            }
        }
    }

    [HarmonyPatch(typeof(TraitSet), "RemoveTrait")]
    public static class TraitSet_RemoveTrait_Patch
    {
        private static void Postfix(Pawn ___pawn, Trait trait)
        {
            if (trait.def is ClassTraitDef traitDef)
            {
                var comp = ___pawn.GetComp<CompPawnClass>();
                comp.Erase(traitDef);
            }
        }
    }
}
