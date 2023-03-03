using Ionic.Zlib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace HRF
{
    public struct AbilityLearnState
    {
        public AbilityDef abilityDef;
        public bool learned;
    }
    [HotSwappable]
	[StaticConstructorOnStartup]
	public static class Utils
	{
		static Utils()
		{
			foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Humanlike ?? false))
			{
                thingDef.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Resource)));
                thingDef.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Class)));
                thingDef.comps.Add(new CompProperties_PawnClass());
			}
		}

		public static bool HasPawnClassComp(this Pawn pawn, out CompPawnClass comp)
		{
			comp = pawn.TryGetComp<CompPawnClass>();
			return comp != null;
        }
        public static IEnumerable<AbilityLearnState> GetAbilities(this Pawn pawn)
		{
            var comp = pawn.TryGetComp<CompPawnClass>();
			var traitDef = comp.ClassTraitDef;
			var unlockedTrees = new List<AbilityTreeDef>();
			foreach (var tree in traitDef.classAbilities)
			{
				var allAbilitiesFromTier = tree.abilityTiers.Select(x => x.abilityDef);
				var existingAbility = allAbilitiesFromTier.FirstOrDefault(x => comp.learnedAbilities.Contains(x));
				if (existingAbility != null)
				{
                    yield return new AbilityLearnState { abilityDef = existingAbility, learned = true };
                }
				else
				{
                    yield return new AbilityLearnState { abilityDef = tree.abilityTiers[0].abilityDef, learned = false };
                }
			}
        }

        public static AbilityDef GetLearnableAbility(this AbilityLearnState learnedState, AbilityData abilityData)
        {
            if (!learnedState.learned)
            {
                return learnedState.abilityDef;
            }
            return abilityData.abilityTree.abilityTiers.Last() != abilityData.abilityTier
                ? abilityData.abilityTree.abilityTiers[abilityData.abilityTree.abilityTiers
                .IndexOf(abilityData.abilityTier) + 1].abilityDef : null;
        }
        public static void TryAssignNewSkillRelatedHediffs(SkillRecord skillRecord, Pawn pawn)
		{
			var options = skillRecord.def.GetModExtension<SkillHediffGrantOptions>();
			if (options != null)
			{
				foreach (var skillGrantOption in options.hediffGrantRequirements)
				{
					var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(skillGrantOption.hediff);
					if (skillGrantOption.minLevel >= skillRecord.Level && skillGrantOption.minPassion >= skillRecord.passion && HasRequiredSkills(pawn, skillGrantOption))
					{
						if (hediff is null)
						{
							hediff = HediffMaker.MakeHediff(skillGrantOption.hediff, pawn);
							pawn.health.AddHediff(hediff);
						}
					}
					else if (hediff != null && !HasRequiredSkills(pawn, skillGrantOption))
					{
						pawn.health.RemoveHediff(hediff);
					}
				}
			}
		}

		private static bool HasRequiredSkills(Pawn pawn, SkillBonusRequirement skillBonusRequirement)
		{
			if (skillBonusRequirement.requiredSkills != null)
			{
				foreach (var requiredSkill in skillBonusRequirement.requiredSkills)
				{
					var skillRecord = pawn.skills.GetSkill(requiredSkill.skill);
					if (skillRecord.Level < requiredSkill.minLevel || skillRecord.passion < requiredSkill.minPassion)
					{
						return false;
					}
				}
			}
			return true;
		}
		public static bool CanDrink(Pawn pawn, Thing potion, out string reason, out bool preventFromUsage)
		{
			var comp = potion.def?.ingestible?.outcomeDoers?.OfType<IngestionOutcomeDoer_GiveHediffResource>().FirstOrDefault();
			HRFLog.Message("Comp: " + comp);
			if (comp?.blacklistHediffsPreventAdd != null)
			{
				foreach (var hediff in comp.blacklistHediffsPreventAdd)
				{
                    HRFLog.Message(pawn + " hediff " + hediff);

                    if (pawn.health.hediffSet.GetFirstHediffOfDef(hediff) != null)
					{
						HRFLog.Message(pawn + " can't drink " + potion);
						reason = comp.cannotDrinkReason;
						preventFromUsage = comp.preventFromUsageIfHasBlacklistedHediff;
                        return false;
					}
				}
			}
			preventFromUsage = false;
            reason = "";
			return true;
		}
		public static bool CanWear(Pawn pawn, Apparel apparel, out string reason)
		{
			var hediffComp = apparel.GetComp<CompApparelAdjustHediffs>();
			if (hediffComp?.Props.resourceSettings != null)
			{
				foreach (var option in hediffComp.Props.resourceSettings)
				{
					var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(option.hediff) as HediffResource;
					if (option.disallowEquipIfHediffMissing)
					{
						if (hediff is null)
						{
							reason = option.cannotEquipReason;
							return false;
						}
					}
					if (option.disallowEquipIfOverCapacity && !hediff.CanGainCapacity(option.maxResourceCapacityOffset))
					{
						reason = option.overCapacityReasonKey.Translate(pawn.Named("PAWN"), apparel.Named("THING"));
						return false;
					}

					if (option.blackListHediffsPreventEquipping != null)
					{
						foreach (var hediffDef in option.blackListHediffsPreventEquipping)
						{
							var blackListHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
							if (blackListHediff != null)
							{
								reason = option.cannotEquipReasonIncompatible + hediffDef.label;
								return false;
							}
						}
					}
				}
			}
			reason = "";
			return true;
		}

		public static bool CanEquip(Pawn pawn, ThingWithComps weapon, out string reason)
		{
			var hediffComp = weapon.GetComp<CompWeaponAdjustHediffs>();
			if (hediffComp?.Props.resourceSettings != null)
			{
				foreach (var option in hediffComp.Props.resourceSettings)
				{
					var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(option.hediff) as HediffResource;
					if (option.disallowEquipIfHediffMissing)
					{
						if (hediff is null || hediff.ResourceAmount <= 0)
						{
							reason = option.cannotEquipReason;
							return false;
						}
					}
					if (option.disallowEquipIfOverCapacity && !hediff.CanGainCapacity(option.maxResourceCapacityOffset))
					{
						reason = option.overCapacityReasonKey.Translate(pawn.Named("PAWN"), weapon.Named("THING"));
						return false;
					}

					if (option.blackListHediffsPreventEquipping != null)
					{
						foreach (var hediffDef in option.blackListHediffsPreventEquipping)
						{
							var blackListHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
							if (blackListHediff != null)
							{
								reason = option.cannotEquipReasonIncompatible;
								return false;
							}
						}
					}
				}
			}
			reason = "";
			return true;
		}
		public static bool IsHediffUser(this Pawn pawn)
		{
			return pawn.health.hediffSet.hediffs.Any(x => x is HediffResource);
		}


		private static Dictionary<Thing, CompAdjustHediffs> cachedComps = new Dictionary<Thing, CompAdjustHediffs>();
		public static bool TryGetCompAdjustHediffs(this Thing thing, out CompAdjustHediffs comp)
		{
			if (!cachedComps.TryGetValue(thing, out comp))
			{
				comp = thing.TryGetComp<CompAdjustHediffs>();
			}
			return comp != null;
		}

		private static Dictionary<Pawn, ValueCache<List<IAdjustResource>>> resourceCache = new Dictionary<Pawn, ValueCache<List<IAdjustResource>>>();
		public static List<IAdjustResource> GetAllAdjustResources(this Pawn pawn)
		{
			if (resourceCache.TryGetValue(pawn, out var adjustResourcesCache))
			{
				var adjusters = adjustResourcesCache.value;
				if (Find.TickManager.TicksGame > adjustResourcesCache.updateTick + 60)
				{
					adjusters = GetAdjustResourcesInt(pawn).ToList();
					adjustResourcesCache.Value = adjusters;
				}
				return adjusters;
			}
			else
			{
				var adjusters = GetAdjustResourcesInt(pawn).ToList();
				resourceCache[pawn] = new ValueCache<List<IAdjustResource>>(adjusters);
				return adjusters;
			}
		}

		private static Dictionary<Apparel, CompApparelAdjustHediffs> compApparelAdjustCache = new Dictionary<Apparel, CompApparelAdjustHediffs>();
		private static Dictionary<ThingWithComps, CompWeaponAdjustHediffs> compWeaponAdjustCache = new Dictionary<ThingWithComps, CompWeaponAdjustHediffs>();
		private static Dictionary<Hediff, HediffComp_AdjustHediffs> hediffCompAdjustCache = new Dictionary<Hediff, HediffComp_AdjustHediffs>();
		private static Dictionary<Hediff, HediffComp_AdjustHediffsPerStages> hediffCompAdjustPerStageCache = new Dictionary<Hediff, HediffComp_AdjustHediffsPerStages>();
		private static Dictionary<Pawn, CompTraitsAdjustHediffs> compTraitsAdjustHediffsCache = new Dictionary<Pawn, CompTraitsAdjustHediffs>();
		private static IEnumerable<IAdjustResource> GetAdjustResourcesInt(Pawn pawn)
		{
			var apparels = pawn.apparel?.WornApparel?.ToList();
			if (apparels != null)
			{
				foreach (var apparel in apparels)
				{
					if (!compApparelAdjustCache.TryGetValue(apparel, out CompApparelAdjustHediffs comp))
					{
						comp = apparel.GetComp<CompApparelAdjustHediffs>();
						compApparelAdjustCache[apparel] = comp;
					}
					if (comp != null)
					{
						yield return comp;
					}
				}
			}

			var equipments = pawn.equipment?.AllEquipmentListForReading;
			if (equipments != null)
			{
				foreach (var equipment in equipments)
				{
					if (!compWeaponAdjustCache.TryGetValue(equipment, out CompWeaponAdjustHediffs comp))
					{
						comp = equipment.GetComp<CompWeaponAdjustHediffs>();
						compWeaponAdjustCache[equipment] = comp;
					}
					if (comp != null)
					{
						yield return comp;
					}
				}
			}

			if (pawn.health?.hediffSet?.hediffs != null)
			{
				foreach (var hediff in pawn.health.hediffSet.hediffs)
				{
					if (!hediffCompAdjustCache.TryGetValue(hediff, out HediffComp_AdjustHediffs comp))
					{
						comp = hediff.TryGetComp<HediffComp_AdjustHediffs>();
						hediffCompAdjustCache[hediff] = comp;
					}
					if (comp != null)
					{
						yield return comp;
					}
					if (!hediffCompAdjustPerStageCache.TryGetValue(hediff, out HediffComp_AdjustHediffsPerStages comp2))
					{
						comp2 = hediff.TryGetComp<HediffComp_AdjustHediffsPerStages>();
						hediffCompAdjustPerStageCache[hediff] = comp2;
					}
					if (comp2 != null)
					{
						yield return comp2;
					}

					if (hediff is HediffResource hediffResource)
                    {
						yield return hediffResource;
                    }
				}
			}

			if (!compTraitsAdjustHediffsCache.TryGetValue(pawn, out CompTraitsAdjustHediffs traitComp))
			{
				traitComp = pawn.TryGetComp<CompTraitsAdjustHediffs>();
				compTraitsAdjustHediffsCache[pawn] = traitComp;
			}

			if (traitComp != null)
			{
				yield return traitComp;
			}
		}
		public static float GetHediffResourceCapacityGainFor(Pawn pawn, HediffResourceDef hdDef, out StringBuilder explanation)
		{
			explanation = new StringBuilder();
			float result = 0;
			var comps = GetAllAdjustResources(pawn);
			foreach (var comp in comps)
			{
				var resourceSettings = comp.ResourceSettings;
				if (resourceSettings != null)
				{
					foreach (var option in resourceSettings)
					{
						if (option.hediff == hdDef)
						{
							var gain = GetCapacityFor(comp, option);
							result += gain;
							explanation.AppendLine("ART.CapacityAdjuster".Translate(comp.Parent.Label, gain));
						}
					}
				}
			}
			return result;
		}


		public static float GetCapacityFor(this IAdjustResource props, ResourceProperties resourceProperties)
		{
			var num = 0f;
			if (resourceProperties.maxResourceCapacityOffset != 0)
            {
				if (resourceProperties.qualityScalesCapacityOffset && props.Parent.TryGetQuality(out QualityCategory qc))
				{
					num = resourceProperties.maxResourceCapacityOffset * GetQualityMultiplier(qc);

					var stuff = props.GetStuff();
					if (stuff != null)
					{
						var extension = stuff.GetModExtension<StuffExtension>();
						if (extension != null)
						{
							foreach (var option in extension.resourceSettings)
							{
								if (resourceProperties.hediff == option.hediff)
								{
									num *= option.resourceCapacityFactor;
									num += option.resourceCapacityOffset;
								}
							}
						}
					}

					foreach (var adjustResourceComp in props.GetOtherResources())
                    {
						foreach (var option in adjustResourceComp.ResourceSettings)
                        {
							if (option.hediff == resourceProperties.hediff)
                            {
								if (resourceProperties.hediff == option.hediff)
								{
									num *= option.resourceCapacityFactor;
									num += option.resourceCapacityOffset;
								}
							}
                        }
                    }
				}
				else
				{
					num = resourceProperties.maxResourceCapacityOffset;
				}
			}
			return num;
		}

		public static void RemoveExcessHediffResources(Pawn pawn, IAdjustResource adjuster)
		{
			List<HediffResourceDef> hediffResourcesToRemove = pawn.health.hediffSet.hediffs.OfType<HediffResource>()
					.Select(x => x.def).Where(x => adjuster.ResourceSettings?.Any(y => y.hediff == x) ?? false).ToList();

			var comps = GetAllAdjustResources(pawn);
			foreach (var comp in comps)
			{
				var resourceSettings = comp.ResourceSettings;
				if (comp.Parent != adjuster.Parent && resourceSettings != null)
				{
					foreach (var resourceProperties in resourceSettings)
					{
						if (resourceProperties.addHediffIfMissing && hediffResourcesToRemove.Contains(resourceProperties.hediff))
						{
							hediffResourcesToRemove.Remove(resourceProperties.hediff);
						}
					}
				}
			}

			foreach (var hediffDef in hediffResourcesToRemove)
			{
				var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				if (hediff != null)
				{
					pawn.health.RemoveHediff(hediff);
				}
			}

			if (adjuster.ResourceSettings != null)
			{
				foreach (var resourceSettings in adjuster.ResourceSettings)
				{
					if (resourceSettings.removeHediffsOnDrop != null)
					{
						foreach (var hediffDef in resourceSettings.removeHediffsOnDrop)
						{
							while (pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef) != null)
							{
								var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
								if (hediff != null)
								{
									pawn.health.hediffSet.hediffs.Remove(hediff);
								}
							}
						}
					}
				}
			}
		}

		public static void TryDropExcessHediffGears(Pawn pawn)
		{
			var comps = GetAllAdjustResources(pawn);
			foreach (var comp in comps)
			{
				var resourceSettings = comp.ResourceSettings;
				if (resourceSettings != null)
				{
					foreach (var resourceProperties in resourceSettings)
					{
						var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
						if (hediff != null && resourceProperties.dropIfOverCapacity && hediff.ResourceCapacity < 0)
						{
							comp.Drop();
							if (!resourceProperties.overCapacityReasonKey.NullOrEmpty())
							{
								Messages.Message(resourceProperties.overCapacityReasonKey.Translate(pawn.Named("PAWN"), comp.Parent.Named("THING")), MessageTypeDefOf.CautionInput);
							}
						}
					}
				}
			}
		}

		public static HediffResource AdjustResourceAmount(this Pawn pawn, HediffResourceDef hdDef, float sevOffset, bool addHediffIfMissing, ResourceProperties resourceProperties, BodyPartDef bodyPartDef, bool applyToDamagedPart = false)
		{
			HediffResource hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(hdDef) as HediffResource;
			if (resourceProperties.fixedResourceAmount != -1)
            {
                if (hediffResource != null)
                {
                    sevOffset = resourceProperties.fixedResourceAmount - hediffResource.ResourceAmount;
                }
                else
                {
                    sevOffset = resourceProperties.fixedResourceAmount;
                }
            }

            if (hediffResource != null)
			{
				if (sevOffset > 0 && hediffResource.def.restrictResourceCap && hediffResource.ResourceAmount >= hediffResource.ResourceCapacity)
                {
					return hediffResource;
                }
				hediffResource.ChangeResourceAmount(sevOffset, resourceProperties);
				return hediffResource;
			}
			else if (addHediffIfMissing && (sevOffset >= 0 || hdDef.keepWhenEmpty))
			{
				BodyPartRecord bodyPartRecord = null;
				if (bodyPartDef != null)
				{
					bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault((BodyPartRecord x) => x.def == bodyPartDef);
					if (pawn.health.hediffSet.GetPartHealth(bodyPartRecord) <= 0f && !applyToDamagedPart)
					{
						return null;
					}
				}
				hediffResource = HediffMaker.MakeHediff(hdDef, pawn, bodyPartRecord) as HediffResource;
				pawn.health.AddHediff(hediffResource);
				hediffResource.ChangeResourceAmount(sevOffset, resourceProperties);
				return hediffResource;
			}
			return null;
		}

		public static float GetQualityMultiplier(QualityCategory qualityCategory)
		{
			switch (qualityCategory)
			{
				case QualityCategory.Awful: return 0.8f;
				case QualityCategory.Poor: return 0.9f;
				case QualityCategory.Normal: return 1f;
				case QualityCategory.Good: return 1.15f;
				case QualityCategory.Excellent: return 1.3f;
				case QualityCategory.Masterwork: return 1.55f;
				case QualityCategory.Legendary: return 1.75f;
				default: return 1f;
			}
		}

		public static float GetQualityMultiplierInverted(QualityCategory qualityCategory)
		{
			switch (qualityCategory)
			{
				case QualityCategory.Awful: return 1.75f;
				case QualityCategory.Poor: return 1.55f;
				case QualityCategory.Normal: return 1.3f;
				case QualityCategory.Good: return 1.15f;
				case QualityCategory.Excellent: return 1f;
				case QualityCategory.Masterwork: return 0.9f;
				case QualityCategory.Legendary: return 0.8f;
				default: return 1f;
			}
		}

		public static Dictionary<Pawn, ValueCache<List<HediffResource>>> hediffResourcesCache = new Dictionary<Pawn, ValueCache<List<HediffResource>>>();
		public static List<HediffResource> GetHediffResourcesFor(Pawn pawn)
		{
			if (hediffResourcesCache.TryGetValue(pawn, out var hediffResourceCache))
			{
				var hediffResources = hediffResourceCache.value;
				if (Find.TickManager.TicksGame > hediffResourceCache.updateTick + 30)
				{
					hediffResources = pawn.health.hediffSet.hediffs.OfType<HediffResource>().ToList();
					hediffResourceCache.Value = hediffResources;
				}
				return hediffResources;
			}
			else
			{
				var hediffResources = pawn.health.hediffSet.hediffs.OfType<HediffResource>().ToList();
				hediffResourcesCache[pawn] = new ValueCache<List<HediffResource>>(hediffResources);
				return hediffResources;
			}
		}
		public static bool IsUsableBy(Verb verb, out string disableReason)
		{
			if (verb.CasterPawn is Pawn pawn)
			{
				var hediffResources = GetHediffResourcesFor(pawn);
				foreach (var hediff in hediffResources)
				{
					if (hediff.CurStage is HediffStageResource hediffStageResource && hediffStageResource.ShieldIsActive(pawn))
					{
						if (hediffStageResource.shieldProperties.cannotUseVerbType == VerbType.Both)
						{
							disableReason = "ART.ShieldPreventAllAttack".Translate();
							return false;
						}
						else if (hediffStageResource.shieldProperties.cannotUseVerbType == VerbType.None)
						{
							continue;
						}
						else if (verb.IsMeleeAttack && hediffStageResource.shieldProperties.cannotUseVerbType == VerbType.Melee)
						{
							disableReason = "ART.ShieldPreventMeleeAttack".Translate();
							return false;
						}
						else if (hediffStageResource.shieldProperties.cannotUseVerbType == VerbType.Range)
						{
							disableReason = "ART.ShieldPreventRangeAttack".Translate();
							return false;
						}
					}
				}

				if (verb.verbProps is IResourceProps props)
				{
					if (!IsUsableForProps(pawn, props, out disableReason))
					{
						return false;
					}
				}

				if (verb.tool is IResourceProps props2)
				{
					if (!IsUsableForProps(pawn, props2, out disableReason))
					{
						return false;
					}
				}
			}

			disableReason = "";
			return true;
		}
		public static bool IsUsableForProps(Pawn pawn, IResourceProps props, out string disableReason)
		{
			var resourceSettings = props.ResourceSettings;
			if (resourceSettings != null)
			{
				foreach (var option in resourceSettings)
				{
					if (option.requiredForUse)
					{
						var resourceHediff = pawn.health.hediffSet.GetFirstHediffOfDef(option.hediff) as HediffResource;
						if (option.disableIfMissingHediff)
						{
							bool hediffResourceIsEmptyOrNull = resourceHediff != null ? resourceHediff.ResourceAmount <= 0 : true;
							if (hediffResourceIsEmptyOrNull)
							{
								disableReason = option.disableReasonKey.Translate();
								return false;
							}
						}

						if (option.minimumResourcePerUse != -1f)
						{
							if (resourceHediff != null && resourceHediff.ResourceAmount < option.minimumResourcePerUse)
							{
								disableReason = option.disableReasonKey.Translate();
								return false;
							}
						}
						if (option.disableAboveResource != -1f)
						{
							if (resourceHediff != null && resourceHediff.ResourceAmount > option.disableAboveResource)
							{
								disableReason = option.disableReasonKey.Translate();
								return false;
							}
						}

						if (option.resourcePerUse < 0)
						{
							if (resourceHediff != null)
							{
								var num = resourceHediff.ResourceAmount - -option.resourcePerUse;
								if (num < 0)
								{
									disableReason = option.disableReasonKey.Translate();
									return false;
								}
							}
						}
					}
				}
			}
			disableReason = "";
			return true;
		}

		public static string GetPropsDescriptions(Pawn pawn, IResourceProps props)
		{
			StringBuilder description = new StringBuilder();
			var resourceSettings = props.ResourceSettings;
			if (resourceSettings != null)
			{
				foreach (var option in resourceSettings)
				{
					if (option.requiredForUse)
                    {
                        if (option.resourcePerUse < 0)
                        {
                            description.AppendLine("ART.ConsumesPerUse".Translate(-option.resourcePerUse, option.hediff.label).Colorize(option.hediff.defaultLabelColor));
                        }
                        else if (option.disableIfMissingHediff)
						{
							description.AppendLine("ART.WillBeDisabledIfMissingResource".Translate(option.hediff.label).Colorize(option.hediff.defaultLabelColor));
						}

						if (option.minimumResourcePerUse != -1f)
						{
							description.AppendLine("ART.RequiresMinimumResource".Translate(option.minimumResourcePerUse, option.hediff.label).Colorize(option.hediff.defaultLabelColor));
						}
						if (option.disableAboveResource != -1f)
						{
							description.AppendLine("ART.WillBeDisabledWhenResourceAbove".Translate(option.hediff.label, option.disableAboveResource).Colorize(option.hediff.defaultLabelColor));
						}


					}
				}
			}
			return description.ToString().TrimEndNewlines();
		}
		public static void DisableGizmo(Gizmo gizmo, string disableReason)
		{
			gizmo.Disable(disableReason);
		}

		public static IChargeResource GetCompChargeSourceFor(Pawn pawn, Projectile projectile)
		{
			var equipments = pawn.equipment?.AllEquipmentListForReading;
			if (equipments != null)
			{
				foreach (var equipment in equipments)
				{
					var chargeComp = equipment.GetComp<CompChargeResource>();
					if (chargeComp != null && (chargeComp.ProjectilesWithChargedResource?.ContainsKey(projectile) ?? false))
					{
						return chargeComp;
					}
				}
			}

			var apparels = pawn.apparel?.WornApparel?.ToList();
			if (apparels != null)
			{
				foreach (var apparel in apparels)
				{
					var chargeComp = apparel.GetComp<CompChargeResource>();
					if (chargeComp != null && (chargeComp.ProjectilesWithChargedResource?.ContainsKey(projectile) ?? false))
					{
						return chargeComp;
					}
				}
			}

			if (pawn.health?.hediffSet?.hediffs != null)
			{
				foreach (var hediff in pawn.health.hediffSet.hediffs)
				{
					var chargeComp = hediff.TryGetComp<HediffCompChargeResource>();
					if (chargeComp != null)
					{
						return chargeComp;
					}
				}
			}

			return null;
		}

		public static void ApplyResourceSettings(Thing target, Pawn casterPawn, IResourceProps props)
        {
            ApplyTargetResourceSettings(target, props);
            ApplyResourceSettings(casterPawn, props);
        }
		public static void ApplyResourceSettings(List<Thing> targets, Pawn casterPawn, IResourceProps props)
		{
			foreach (var target in targets)
            {
				ApplyTargetResourceSettings(target, props);
			}
			ApplyResourceSettings(casterPawn, props);
		}


		private static void ApplyTargetResourceSettings(Thing target, IResourceProps props)
        {
            if (props.TargetResourceSettings != null)
            {
                var targetPawn = target as Pawn;
                if (targetPawn != null)
                {
                    foreach (var option in props.TargetResourceSettings)
                    {
                        if (option.resetLifetimeTicks)
                        {
                            var targetHediff = targetPawn.health.hediffSet.GetFirstHediffOfDef(option.hediff) as HediffResource;
                            if (targetHediff != null)
                            {
                                targetHediff.duration = 0;
                            }
                        }
                    }
                }
            }
        }

        private static void ApplyResourceSettings(Pawn casterPawn, IResourceProps props)
        {
            if (props.ResourceSettings != null)
            {
                var hediffResourceManage = HRFManager.Instance;

                var hediffPostUse = new Dictionary<HediffResource, List<int>>();
                var hediffPostUseDelayMultipliers = new Dictionary<HediffResource, List<float>>();

                var disablePostUseString = "";
                var comps = GetAllAdjustResources(casterPawn);

                foreach (var resourceProperties in props.ResourceSettings)
                {
                    var hediffResource = AdjustResourceAmount(casterPawn, resourceProperties.hediff, resourceProperties.resourcePerUse,
                        resourceProperties.addHediffIfMissing, resourceProperties, resourceProperties.applyToPart);
                    if (hediffResource != null)
                    {
                        var hediffResourcePostUseDelay = new List<int>();
                        var hediffResourcePostUseDelayMultipliers = new List<float>();
                        foreach (var comp in comps)
                        {
                            var compResourseSettings = comp.ResourceSettings?.FirstOrDefault(x => x.hediff == resourceProperties.hediff);
                            if (compResourseSettings != null)
                            {
                                if (resourceProperties.postUseDelay != 0)
                                {
                                    hediffResourcePostUseDelay.Add(resourceProperties.postUseDelay);
                                    disablePostUseString += comp.DisablePostUse + "\n";
                                    if (compResourseSettings.postUseDelayMultiplier != 1)
                                    {
                                        hediffResourcePostUseDelayMultipliers.Add(compResourseSettings.postUseDelayMultiplier);
                                    }
                                }
                            }

                            if (resourceProperties.postUseDelay != 0)
                            {
                                if (hediffPostUse.ContainsKey(hediffResource))
                                {
                                    hediffPostUse[hediffResource].Add(resourceProperties.postUseDelay);
                                }
                                else
                                {
                                    hediffPostUse[hediffResource] = new List<int> { resourceProperties.postUseDelay };
                                }
                                if (compResourseSettings != null && compResourseSettings.postUseDelayMultiplier != 1)
                                {
                                    if (hediffPostUseDelayMultipliers.ContainsKey(hediffResource))
                                    {
                                        hediffPostUseDelayMultipliers[hediffResource].Add(compResourseSettings.postUseDelayMultiplier);
                                    }
                                    else
                                    {
                                        hediffPostUseDelayMultipliers[hediffResource] = new List<float> { compResourseSettings.postUseDelayMultiplier };
                                    }
                                }
                            }
                        }

                        if (hediffResourcePostUseDelay.Any() && hediffResourcePostUseDelayMultipliers.Any())
                        {
                            foreach (var comp in comps)
                            {
                                comp.PostUseDelayTicks[hediffResource] = new HediffResouceDisable((int)((Find.TickManager.TicksGame + hediffResourcePostUseDelay.Average()) * hediffResourcePostUseDelayMultipliers.Average()), disablePostUseString);
                            }
                        }
                        else if (hediffResourcePostUseDelay.Any())
                        {
                            foreach (var comp in comps)
                            {
                                comp.PostUseDelayTicks[hediffResource] = new HediffResouceDisable((int)((Find.TickManager.TicksGame + hediffResourcePostUseDelay.Average())), disablePostUseString);
                            }
                        }
                    }
                }

                foreach (var hediffData in hediffPostUse)
                {
                    if (hediffData.Key != null && hediffPostUse.TryGetValue(hediffData.Key, out List<int> hediffPostUseList))
                    {
                        int newDelayTicks;
                        if (hediffPostUseDelayMultipliers.TryGetValue(hediffData.Key, out List<float> hediffPostUseMultipliers) && hediffPostUseMultipliers.Any())
                        {
                            newDelayTicks = (int)(hediffPostUseList.Average() * hediffPostUseMultipliers.Average());
                        }
                        else
                        {
                            newDelayTicks = (int)(hediffPostUseList.Average());
                        }

                        if (hediffData.Key.CanHaveDelay(newDelayTicks))
                        {
                            hediffData.Key.AddDelay(newDelayTicks);
                        }
                    }
                }
            }
        }

        public static void ApplyChargeResource(ref float damageAmount, ChargeResources chargeResources)
		{
			HRFLog.Message("Old damage: " + damageAmount);
			foreach (var chargeResource in chargeResources.chargeResources)
			{
				switch (chargeResource.chargeSettings.damageScaling)
				{
					case DamageScalingMode.Flat: DoFlatDamage(ref damageAmount, chargeResource.chargeResource, chargeResource.chargeSettings); break;
					case DamageScalingMode.Scalar: DoScalarDamage(ref damageAmount, chargeResource.chargeResource, chargeResource.chargeSettings); break;
					case DamageScalingMode.Linear: DoLinearDamage(ref damageAmount, chargeResource.chargeResource, chargeResource.chargeSettings); break;
					default: break;
				}
			}
			HRFLog.Message("New damage: " + damageAmount);
		}

		private static void DoFlatDamage(ref float __result, float resourceAmount, ChargeSettings chargeSettings)
		{
			__result = (int)(__result + (chargeSettings.damagePerCharge * (resourceAmount - chargeSettings.minimumResourcePerUse) / chargeSettings.resourcePerCharge));
		}
		private static void DoScalarDamage(ref float __result, float resourceAmount, ChargeSettings chargeSettings)
		{
			__result = (int)(__result * Mathf.Pow((1 + chargeSettings.damagePerCharge), (resourceAmount - chargeSettings.minimumResourcePerUse) / chargeSettings.resourcePerCharge));
		}

		private static void DoLinearDamage(ref float __result, float resourceAmount, ChargeSettings chargeSettings)
		{
			__result = (int)(__result * (1 + (chargeSettings.damagePerCharge * (resourceAmount - chargeSettings.minimumResourcePerUse) / chargeSettings.resourcePerCharge)));
		}

		public static HashSet<IntVec3> GetAllCellsAround(ResourceProperties option, TargetInfo targetInfo, CellRect occupiedCells)
		{
			if (option.worksThroughWalls)
			{
				return GetAllCellsInRadius(option, occupiedCells);
			}
			else
			{
				return GetAffectedCells(option, targetInfo, occupiedCells);
			}
		}
		public static HashSet<IntVec3> GetAllCellsInRadius(ResourceProperties option, CellRect occupiedCells)
		{
			HashSet<IntVec3> tempCells = new HashSet<IntVec3>();
			foreach (var cell in occupiedCells)
			{
				foreach (var intVec in GenRadial.RadialCellsAround(cell, option.effectRadius, true))
				{
					tempCells.Add(intVec);
				}
			}
			return tempCells;
		}
		public static HashSet<IntVec3> GetAffectedCells(ResourceProperties option, TargetInfo targetInfo, CellRect occupiedCells)
		{
			HashSet<IntVec3> affectedCells = new HashSet<IntVec3>();
			HashSet<IntVec3> tempCells = GetAllCellsInRadius(option, occupiedCells);
			Predicate<IntVec3> validator = delegate (IntVec3 cell)
			{
				if (!tempCells.Contains(cell)) return false;
				var edifice = cell.GetEdifice(targetInfo.Map);
				var result = edifice == null || edifice.def.passability != Traversability.Impassable || occupiedCells.Cells.Contains(cell);
				return result;
			};
			var centerCell = occupiedCells.CenterCell;
			targetInfo.Map.floodFiller.FloodFill(centerCell, validator, delegate (IntVec3 x)
			{
				if (tempCells.Contains(x))
				{
					var edifice = x.GetEdifice(targetInfo.Map);
					var result = edifice == null || edifice.def.passability != Traversability.Impassable || occupiedCells.Cells.Contains(x);
					if (result && (GenSight.LineOfSight(centerCell, x, targetInfo.Map) || centerCell.DistanceTo(x) <= 1.5f))
					{
						affectedCells.Add(x);
					}
				}
			}, int.MaxValue, rememberParents: false, (IEnumerable<IntVec3>)null);
			affectedCells.AddRange(occupiedCells.Cells);
			return affectedCells;
		}

		public static float GetResourceGain(this ResourceProperties resourceProperties, IAdjustResource source = null)
		{
			float num = resourceProperties.resourcePerSecond;
			if (source != null && resourceProperties.qualityScalesResourcePerSecond && source.TryGetQuality(out QualityCategory qc))
			{
				num *= GetQualityMultiplier(qc);
			}
			var stuff = source?.GetStuff();
			if (stuff != null)
            {
				var extension = stuff.GetModExtension<StuffExtension>();
				if (extension != null)
                {
					foreach (var option in extension.resourceSettings)
                    {
						if (option.hediff == resourceProperties.hediff)
                        {
							num *= option.resourcePerSecondFactor;
							num += option.resourcePerSecondOffset;
						}
                    }
				}
            }

			if (source != null)
            {
				foreach (var otherComp in source.GetOtherResources())
				{
					foreach (var option in otherComp.ResourceSettings)
					{
						if (option.hediff == resourceProperties.hediff)
						{
							num *= option.resourcePerSecondFactor;
							num += option.resourcePerSecondOffset;
						}
					}
				}
			}

			if (resourceProperties.refillOnlyInnerStorage)
			{
				return num;
			}
			else
			{
				return num * resourceProperties.hediff.resourcePerSecondFactor;
			}
		}

		public static IEnumerable<IAdjustResource> GetOtherResources(this IAdjustResource comp)
        {
			var pawnHost = comp.PawnHost;
			if (pawnHost != null)
            {
				foreach (var otherResourceComp in pawnHost.GetAllAdjustResources())
                {
					if (otherResourceComp != comp)
                    {
						yield return otherResourceComp;
                    }
                }
            }
        }
		public static IResourceProps GetResourceProps(this Verb verb)
		{
			if (verb.verbProps is IResourceProps verbResourceProps) return verbResourceProps;
			if (verb.tool is IResourceProps toolResourceProps) return toolResourceProps;
			return null;
		}

		public static bool HasResource(this Pawn pawn, HediffResourceDef hediffResource, float minResource = 0f)
        {
			var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffResource) as HediffResource;
			if (hediff != null && hediff.ResourceAmount >= minResource)
            {
				return true;
            }
			return false;
        }

		public static IEnumerable<Pawn> GetPawnsAround(Pawn checker, GeneralProperties properties)
		{
			if (checker.MapHeld is null)
            {
				yield break;
            }
			else
			{
				if (properties.effectRadius > 0)
				{
					foreach (var pawn in GetPawnsAround(checker, properties.effectRadius))
					{
						yield return pawn;

                    }
                }
				else if (properties.affectsSelf)
				{
					yield return checker;
				}
			}
		}

		public static IEnumerable<Pawn> GetPawnsAround(Pawn checker, float effectRadius)
		{
            foreach (var pawn in GenRadial.RadialDistinctThingsAround(checker.PositionHeld, checker.MapHeld, effectRadius, true).OfType<Pawn>())
            {
                yield return pawn;
            }
        }
		public static void HealHediffs(Pawn pawn, ref float healPoints, List<Hediff> hediffsToHeal, bool pointsOverflow, HealPriority healPriority, bool healAll, SoundDef soundOnEffect)
		{
			if (healPriority == HealPriority.TendablesFirst)
			{
				hediffsToHeal = hediffsToHeal.OrderBy(x => x.TendableNow() ? 0 : 1).ToList();
			}
			else
			{
				hediffsToHeal = hediffsToHeal.InRandomOrder().ToList();
			}
			foreach (var hediff in hediffsToHeal)
			{
				if (healAll)
				{
					hediff.Severity = 0;
                    if (soundOnEffect != null)
					{
						soundOnEffect.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					}
				}
				else if (healPoints != 0)
				{
					var toHealPoints = Mathf.Min(healPoints, hediff.Severity);
					hediff.Severity -= toHealPoints;
					healPoints -= toHealPoints;
					if (soundOnEffect != null)
					{
						soundOnEffect.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					}
				}

				if (!healAll && (!pointsOverflow || healPoints == 0))
				{
					return;
				}
			}
		}
	}
}