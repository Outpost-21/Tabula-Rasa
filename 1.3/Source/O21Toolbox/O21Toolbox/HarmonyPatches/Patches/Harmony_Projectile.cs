using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using O21Toolbox.Projectiles;
using System.Reflection;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_Projectile
    {
	}

	[HarmonyPatch(typeof(Projectile))]
	[HarmonyPatch("Launch")]
	[HarmonyPatch(new Type[]
	{
		typeof(Thing),
		typeof(Vector3),
		typeof(LocalTargetInfo),
		typeof(LocalTargetInfo),
		typeof(ProjectileHitFlags),
		typeof(bool),
		typeof(Thing),
		typeof(ThingDef)
	})]
	public class Patch_Projectile_Launch
	{
		[HarmonyPostfix]
		public static void Postfix(Projectile __instance, Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, bool preventFriendlyFire, Thing equipment, ThingDef targetCoverDef)
		{
			Comp_Homing comp_Homing = __instance.TryGetComp<Comp_Homing>();
			if (comp_Homing != null)
			{
				float homingChance = comp_Homing.Props.chance;
				if (Rand.Chance(homingChance))
				{
					if (usedTarget != intendedTarget)
					{
						Vector3 vector = intendedTarget.Cell.ToVector3Shifted();
						AccessTools.Field(typeof(Projectile), "destination").SetValue(__instance, vector);
						AccessTools.Field(typeof(Projectile), "ticksToImpact").SetValue(__instance, Mathf.CeilToInt(RecalculateTicksToImpact(__instance, vector)));
						__instance.HitFlags = (ProjectileHitFlags.IntendedTarget | ProjectileHitFlags.NonTargetPawns);
						__instance.usedTarget = intendedTarget;
					}
					if (intendedTarget.HasThing && !intendedTarget.ThingDestroyed)
					{
						comp_Homing.IsHoming = true;
						comp_Homing.LastCell = intendedTarget.Thing.Position;
					}
				}
			}
		}

		public static float RecalculateTicksToImpact(Projectile projectile, Vector3 newDestination)
		{
			return (projectile.ExactPosition - newDestination).magnitude / projectile.def.projectile.SpeedTilesPerTick;
		}
	}

	[HarmonyPatch(typeof(Projectile))]
	[HarmonyPatch("Tick")]
	public class PatchProjectileTick
	{
		[HarmonyPrefix]
		public static bool Prefix(Projectile __instance)
		{
			LocalTargetInfo intendedTarget = __instance.intendedTarget;
			Comp_Homing comp_homing;
			if (intendedTarget.HasThing && !intendedTarget.ThingDestroyed)
			{
				comp_homing = __instance.TryGetComp<Comp_Homing>();
				if (comp_homing != null)
				{
					if (comp_homing.IsHoming && !intendedTarget.Thing.Position.Equals(comp_homing.lastCell))
					{
						FieldInfo ticksToImpact = AccessTools.Field(typeof(Projectile), "ticksToImpact");
						FieldInfo destination = AccessTools.Field(typeof(Projectile), "destination");
						FieldInfo origin = AccessTools.Field(typeof(Projectile), "origin");
						if (comp_homing.ShouldRecalculateNow())
						{
							Vector3 vector = intendedTarget.Thing.Position.ToVector3Shifted();
							float magnitude = ((Vector3)destination.GetValue(__instance) - vector).magnitude;
							origin.SetValue(__instance, __instance.ExactPosition);
							destination.SetValue(__instance, vector);
							ticksToImpact.SetValue(__instance, Mathf.CeilToInt(RecalculateTicksToImpact(__instance, vector)));
							comp_homing. LastCell = intendedTarget.Thing.Position;
						}
					}
				}
			}
			return true;
		}

		public static float RecalculateTicksToImpact(Projectile projectile, Vector3 newDestination)
		{
			return (projectile.ExactPosition - newDestination).magnitude / projectile.def.projectile.SpeedTilesPerTick;
		}
	}
}
