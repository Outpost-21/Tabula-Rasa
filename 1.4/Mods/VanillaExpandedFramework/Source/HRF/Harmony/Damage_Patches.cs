using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using RimWorld.Planet;
using RuntimeAudioClipLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using static Verse.DamageWorker;

namespace HRF
{
	[HarmonyPatch(typeof(Projectile), "Launch", new Type[]
	{
		typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef)
	})]
	public static class Patch_Projectile_Launch
	{
		public static void Postfix(Projectile __instance, Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null, ThingDef targetCoverDef = null)
		{
			if (equipment != null)
            {
				HRFManager.Instance.firedProjectiles[__instance] = new FiredData
				{
					caster = launcher,
					equipment = equipment,
				};
			}

			if (launcher is Pawn pawn && Patch_TryCastShot.verbSource != null)
			{
				var compCharge = GetChargeSourceFrom(Patch_TryCastShot.verbSource, pawn);
				if (compCharge != null)
				{
					var verbProps = Patch_TryCastShot.verbSource.GetVerb.verbProps as VerbResourceProps;
					if (verbProps?.chargeSettings != null)
					{
						foreach (var chargeSettings in verbProps.chargeSettings)
						{
							var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(chargeSettings.hediffResource) as HediffResource;
							if (hediffResource != null && chargeSettings.damageScaling.HasValue)
							{
								if (compCharge.ProjectilesWithChargedResource.ContainsKey(__instance))
								{
									compCharge.ProjectilesWithChargedResource[__instance].chargeResources.Add(new ChargeResource(hediffResource.ResourceAmount, chargeSettings));
								}
								else
								{
									compCharge.ProjectilesWithChargedResource[__instance] = new ChargeResources();
									compCharge.ProjectilesWithChargedResource[__instance].chargeResources = new List<ChargeResource> { new ChargeResource(hediffResource.ResourceAmount, chargeSettings) };
								}
								hediffResource.SetResourceAmount(0f, null);
							}
						}
					}
				}
			}
		}

		private static IChargeResource GetChargeSourceFrom(Verb verb, Pawn pawn)
		{
			if (verb.EquipmentSource != null) return verb.EquipmentSource.GetComp<CompChargeResource>();
			if (verb.HediffCompSource != null) return verb.HediffSource.TryGetComp<HediffCompChargeResource>();
			return null;
		}
	}

	[HarmonyPatch(typeof(Projectile), "DamageAmount", MethodType.Getter)]
	internal static class DamageAmount_Patch
	{
		private static void Postfix(Projectile __instance, ref int __result)
		{
			if (__instance.Launcher is Pawn launcher)
			{
				var compCharge = Utils.GetCompChargeSourceFor(launcher, __instance);
				if (compCharge?.ProjectilesWithChargedResource != null && compCharge.ProjectilesWithChargedResource.TryGetValue(__instance, out ChargeResources chargeResources) && chargeResources != null)
				{
					var amount = (float)__result;
					Utils.ApplyChargeResource(ref amount, chargeResources);
					__result = (int)amount;
					compCharge.ProjectilesWithChargedResource.Remove(__instance);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Bullet), "Impact")]
	public static class Impact_Patch
	{
		public static Thing hitThingStatic;
		public static Projectile curProjectileStatic;

		private static void Prefix(Projectile __instance, Thing hitThing)
		{
			hitThingStatic = hitThing;
			curProjectileStatic = __instance;
		}

		private static void Postfix(Projectile __instance, Thing hitThing)
		{
			hitThingStatic = null;
			curProjectileStatic = null;
		}

		public static void ImpactThing(Projectile __instance, Thing hitThing, DamageInfo source)
        {
			if (hitThing != null)
			{
				if (HRFManager.Instance.firedProjectiles.TryGetValue(__instance, out var firedData))
                {
					var targetPawn = hitThing as Pawn;
					if (targetPawn != null)
					{
						var extension = firedData.equipment?.def.GetModExtension<ResourceOnActionExtension>();
						if (extension != null)
						{
							foreach (var resourceOnAction in extension.resourcesOnAction)
							{
								if (!resourceOnAction.onSelf)
								{
									resourceOnAction.TryApplyOn(targetPawn);
								}
							}
						}

						if (firedData.caster is Pawn pawn)
						{
							foreach (var hediff in pawn.health.hediffSet.hediffs)
							{
								var extension2 = hediff.def.GetModExtension<ResourceOnActionExtension>();
								if (extension2 != null)
								{
									foreach (var resourceOnAction in extension2.resourcesOnAction)
									{
										if (!resourceOnAction.onSelf)
										{
											resourceOnAction.TryApplyOn(targetPawn);
										}
									}
								}
							}
						}
					}

					var stuffExtension = firedData.equipment?.Stuff?.GetModExtension<StuffExtension>();
					if (stuffExtension != null)
					{
						stuffExtension.DamageThing(firedData.caster, hitThing, source, true, false);
					}

					HRFLog.Message("Projectile fire data: __instance: " + __instance + " - hitThing: " + hitThing + " - source: " + source + " - caster: " + firedData.caster);

					if (firedData.caster is Pawn instigator)
					{
						HRFLog.Message("Iterating over pawn: " + instigator);
						if (instigator.health?.hediffSet.hediffs != null)
						{
							HRFLog.Message("Checking hediffs on: " + instigator);
							for (int i = instigator.health.hediffSet.hediffs.Count - 1; i >= 0; i--)
							{
								var hediff = instigator.health.hediffSet.hediffs[i];
								HRFLog.Message("Checking hediff on: " + hediff);
								if (hediff is HediffResource hediffResource && hediffResource.CurStage is HediffStageResource hediffStageResource)
								{
									HRFLog.Message("Passed hediff: " + hediff);
									if (hediffStageResource.additionalDamages != null)
									{
										foreach (var additionalDamage in hediffStageResource.additionalDamages)
										{
											HRFLog.Message("Looking at : " + additionalDamage.damage + " - " + additionalDamage.damageRange);
											if (additionalDamage.damageRange)
											{
												var damageAmount = additionalDamage.amount.RandomInRange;
												var damage = new DamageInfo(additionalDamage.damage, damageAmount, instigator: source.Instigator, hitPart: source.HitPart, weapon: source.Weapon);
												HRFLog.Message(hitThing + " should take damage: " + damage);
												hitThing.TakeDamage(damage);
												HRFLog.Message(hitThing + " 2 should take damage: " + damage);
											}
											else
											{
												HRFLog.Message(hitThing + " won't take damage: " + additionalDamage.damage);
											}
										}
									}
									if (targetPawn != null && hediffStageResource.lifeStealProperties != null && hediffStageResource.lifeStealProperties.affectRanged)
									{
										hediffStageResource.lifeStealProperties.StealLife(instigator, targetPawn, source);
									}
								}
							}
						}
					}
				}
				else
				{
					HRFLog.Error("Projectile fire data isn't found: __instance: " + __instance + " - hitThing: " + hitThing + " - source: " + source);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Thing), "TakeDamage")]
	public static class Patch_TakeDamage
    {
        public static bool Prefix(ref DamageWorker.DamageResult __result, Thing __instance, DamageInfo dinfo)
        {
            if (__instance is Pawn pawn && pawn.health.hediffSet.hediffs.Any(x => x.CurStage is HediffStageResource hediffStageResource && hediffStageResource.preventDamage))
            {
				__result = new DamageResult();
                return false;
            }
            return true;
        }
        public static bool preventRecursion;
		public static void Postfix(Thing __instance, DamageInfo dinfo)
		{
			if (!preventRecursion && Impact_Patch.hitThingStatic != null && __instance == Impact_Patch.hitThingStatic && Impact_Patch.curProjectileStatic != null)
            {
				preventRecursion = true;
				Impact_Patch.ImpactThing(Impact_Patch.curProjectileStatic, Impact_Patch.hitThingStatic, dinfo);
				preventRecursion = false;
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "PreApplyDamage")]
	public static class Patch_PreApplyDamage
	{
		private static void Prefix(Pawn __instance, ref DamageInfo dinfo, out bool absorbed)
		{
			absorbed = false;
			var effectOnImpactOptions = dinfo.Def.GetModExtension<EffectOnImpact>();
			if (effectOnImpactOptions != null && __instance.health?.hediffSet != null)
			{
				foreach (var resourceEffect in effectOnImpactOptions.resourceEffects)
				{
					var hediffResource = Utils.AdjustResourceAmount(__instance, resourceEffect.hediff,
						resourceEffect.adjustTargetResource, resourceEffect.addHediffIfMissing, resourceEffect, resourceEffect.applyToPart);
					if (hediffResource != null && resourceEffect.delayTargetOnDamage != IntRange.zero)
					{
						hediffResource.AddDelay(resourceEffect.delayTargetOnDamage.RandomInRange);
					}
				}
			}

			var hediffResources = __instance?.health?.hediffSet?.hediffs?.OfType<HediffResource>().ToList();
			if (hediffResources != null)
			{
				for (int num = hediffResources.Count - 1; num >= 0; num--)
				{
					var hediff = hediffResources[num];
					if (dinfo.Amount > 0 && hediff.CurStage is HediffStageResource hediffStageResource && hediffStageResource.ShieldIsActive(__instance))
					{
						var shieldProps = hediffStageResource.shieldProperties;
						if (shieldProps.absorbRangeDamage && (dinfo.Weapon?.IsRangedWeapon ?? false))
						{
							ProcessDamage(__instance, ref dinfo, hediff, shieldProps);
						}
						else if (shieldProps.absorbMeleeDamage && (dinfo.Weapon is null || dinfo.Weapon == ThingDefOf.Human || dinfo.Weapon.IsMeleeWeapon))
						{
							ProcessDamage(__instance, ref dinfo, hediff, shieldProps);
						}
						if (dinfo.Amount <= 0)
						{
							absorbed = true;
						}
						hediff.AbsorbedDamage(dinfo);
					}
				}
			}
		}

		private static void ProcessDamage(Pawn pawn, ref DamageInfo dinfo, HediffResource hediff, ShieldProperties shieldProps)
		{
			bool damageIsProcessed = false;
			if (shieldProps.resourceConsumptionPerDamage.HasValue && hediff.ResourceAmount >= shieldProps.resourceConsumptionPerDamage.Value)
			{
				if (shieldProps.maxAbsorb.HasValue)
				{
					dinfo.SetAmount(dinfo.Amount - shieldProps.maxAbsorb.Value);
				}
				else
				{
					dinfo.SetAmount(0);
				}
				hediff.ChangeResourceAmount(-shieldProps.resourceConsumptionPerDamage.Value, null);
				damageIsProcessed = true;
			}
			else if (shieldProps.damageAbsorbedPerResource.HasValue)
			{
				var damageAmount = dinfo.Amount;
				if (shieldProps.maxAbsorb.HasValue && damageAmount > shieldProps.maxAbsorb.Value)
				{
					damageAmount = shieldProps.maxAbsorb.Value;
				}
				var resourceAmount = hediff.ResourceAmount;
				var ratioPerAbsorb = shieldProps.damageAbsorbedPerResource.Value;
				var resourceCost = damageAmount / ratioPerAbsorb;
				if (resourceAmount >= resourceCost)
				{
					dinfo.SetAmount(0f);
					hediff.ChangeResourceAmount(-resourceCost, null);
				}
				else
				{
					damageAmount -= resourceAmount * ratioPerAbsorb;
					dinfo.SetAmount(damageAmount);
					hediff.SetResourceAmount(0, null);
				}
				damageIsProcessed = true;
			}

			if (damageIsProcessed && shieldProps.postDamageDelay.HasValue)
			{
				var apparels = pawn.apparel?.WornApparel?.ToList();
				if (apparels != null)
				{
					foreach (var apparel in apparels)
					{
						var hediffComp = apparel.GetComp<CompAdjustHediffs>();
						if (hediffComp != null && hediffComp.Props.resourceSettings != null)
						{
							foreach (var resourceProperties in hediffComp.Props.resourceSettings)
							{
								var newDelayTicks = (int)(shieldProps.postDamageDelay.Value * resourceProperties.postDamageDelayMultiplier);
								var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
								if (hediffResource != null && hediffResource.CanHaveDelay(newDelayTicks))
								{
									hediffResource.AddDelay(newDelayTicks);
								}
							}
						}
					}
				}

				var equipments = pawn.equipment?.AllEquipmentListForReading;
				if (equipments != null)
				{
					foreach (var equipment in equipments)
					{
						var hediffComp = equipment.GetComp<CompAdjustHediffs>();
						if (hediffComp != null && hediffComp.Props.resourceSettings != null)
						{
							foreach (var resourceProperties in hediffComp.Props.resourceSettings)
							{
								var newDelayTicks = (int)(shieldProps.postDamageDelay.Value * resourceProperties.postDamageDelayMultiplier);
								var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
								if (hediffResource != null && hediffResource.CanHaveDelay(newDelayTicks))
								{
									hediffResource.AddDelay(newDelayTicks);
								}
							}
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(ThingWithComps), "Destroy")]
	public static class Patch_Destroy
	{
		public static void Prefix(ThingWithComps __instance)
		{
			if (__instance.comps != null)
            {
				foreach (var comp in __instance.comps)
				{
					if (comp is IAdjustResource adjustResource)
                    {
						adjustResource.Notify_Removed();
					}
				}
			}
		}
	}

    [HarmonyPatch(typeof(DamageResult), "AssociateWithLog")]
    public static class DamageResult_AssociateWithLog
    {
        public static void Prefix(LogEntry_DamageResult log)
        {
            if (log is BattleLogEntry_RangedImpact battleLog && battleLog.weaponDef is null)
            {
                battleLog.weaponDef = battleLog.initiatorPawn.def;
            }
        }
    }
}
