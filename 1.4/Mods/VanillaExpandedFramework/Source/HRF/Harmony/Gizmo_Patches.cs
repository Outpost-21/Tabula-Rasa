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
    [HarmonyPatch(typeof(CompReloadable), "CreateVerbTargetCommand")]
	public static class Patch_CreateVerbTargetCommand
	{
		private static void Postfix(ref Command_Reloadable __result, Thing gear, Verb verb)
		{
			if (__result != null && verb.CasterIsPawn)
			{
				if (!Utils.IsUsableBy(verb, out string disableReason))
				{
					Utils.DisableGizmo(__result, disableReason);
				}
				var resourceProps = verb.GetResourceProps();
				if (resourceProps != null)
				{
					__result.defaultDesc += "\n" + Utils.GetPropsDescriptions(verb.CasterPawn, resourceProps);
				}
			}
		}
	}

	[HarmonyPatch(typeof(PawnVerbGizmoUtility), "GetGizmosForVerb")]
	public static class Patch_GetGizmosForVerb
	{
		private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Verb verb)
		{
			bool isUsable = Utils.IsUsableBy(verb, out string disableReason);
			foreach (var gizmo in __result)
            {
				if (!isUsable)
				{
					Utils.DisableGizmo(gizmo, disableReason);
				}
				if (gizmo is Command command)
                {
					var resourceProps = verb.GetResourceProps();
					if (resourceProps != null)
                    {
						command.defaultDesc += "\n" + Utils.GetPropsDescriptions(verb.CasterPawn, resourceProps);
					}
				}
				yield return gizmo;
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public class Pawn_GetGizmos_Patch
	{
		public static Dictionary<HediffResource, Gizmo_ResourceStatus> cachedGizmos = new Dictionary<HediffResource, Gizmo_ResourceStatus>();
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			foreach (var g in __result)
            {
				yield return g;
            }
			if (__instance.Faction == Faction.OfPlayerSilentFail)
			{
				var hediffResources = Utils.GetHediffResourcesFor(__instance);
				if (hediffResources != null)
                {
					foreach (var hediffResource in hediffResources)
                    {
						if (hediffResource.def.showResourceBar)
                        {
							if (cachedGizmos.TryGetValue(hediffResource, out Gizmo_ResourceStatus gizmo_ResourceStatus))
                            {
								yield return gizmo_ResourceStatus;
							}
							else
                            {
								gizmo_ResourceStatus = new Gizmo_ResourceStatus(hediffResource);
								cachedGizmos[hediffResource] = gizmo_ResourceStatus;
								yield return gizmo_ResourceStatus;
							}
						}
					}
                }
			}

			if (Prefs.DevMode)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "Debug: Charge all hediff resources";
				command_Action.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.SetResourceAmount(hediffResource.ResourceCapacity + hediffResource.StoragesTotalCapacity);
						}
					}
				};
				yield return command_Action;

				Command_Action command_Empty = new Command_Action();
				command_Empty.defaultLabel = "Debug: Empty all hediff resources";
				command_Empty.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.SetResourceAmount(0);
						}
					}
				};
				yield return command_Empty;

				Command_Action command_SetTo10= new Command_Action();
				command_SetTo10.defaultLabel = "Debug: Set all hediff resources to 10";
				command_SetTo10.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.SetResourceAmount(10);
						}
					}
				};
				yield return command_SetTo10;

				Command_Action command_ReduceBy10 = new Command_Action();
				command_ReduceBy10.defaultLabel = "Debug: Reduce all hediff resources by 10";
				command_ReduceBy10.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.ChangeResourceAmount(-10, null);
						}
					}
				};
				yield return command_ReduceBy10;

				Command_Action command_AddBy10 = new Command_Action();
				command_AddBy10.defaultLabel = "Debug: Add all hediff resources by 10";
				command_AddBy10.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.ChangeResourceAmount(10, null);
						}
					}
				};
				yield return command_AddBy10;

				Command_Action command_Action3 = new Command_Action();
				command_Action3.defaultLabel = "Debug: Remove all hediff resources";
				command_Action3.action = delegate
				{
					var hediffResources = Utils.GetHediffResourcesFor(__instance);
					if (hediffResources != null)
					{
						foreach (var hediffResource in hediffResources)
						{
							hediffResource.pawn.health.RemoveHediff(hediffResource);
						}
					}
				};
				yield return command_Action3;

                var comp = __instance.GetComp<CompPawnClass>();
                if (comp != null)
                {
                    if (comp.HasClass(out _))
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = "DEV: Gain 10 xp",
                            action = delegate
                            {
                                comp.GainXP(10f);
                            }
                        };
                        yield return new Command_Action
                        {
                            defaultLabel = "DEV: Gain 100 xp",
                            action = delegate
                            {
                                comp.GainXP(100f);
                            }
                        };
                        yield return new Command_Action
                        {
                            defaultLabel = "DEV: Gain 1000 xp",
                            action = delegate
                            {
                                comp.GainXP(1000f);
                            }
                        };
                    }
                }
            }
		}
	}

	//[HarmonyPatch(typeof(Command), "GizmoOnGUI")]
	//public static class Patch_GizmoOnGUI
	//{
	//	private static void Postfix(Command __instance, Vector2 topLeft, float maxWidth)
	//	{
	//		if (__instance is Command_VerbTarget verbTarget)
    //        {
	//			if (TryGetAmmoString(verbTarget.verb, out List<Tuple<ResourceProperties, HediffResource>> hediffs))
	//			{
	//				var butRect = new Rect(topLeft.x, topLeft.y, verbTarget.GetWidth(maxWidth), 75f);
	//				for (var i = 0; i < hediffs.Count; i++)
    //                {
	//					var pos = 35f - ((i + 1) * 18f);
	//					Vector2 vector = new Vector2(5f, pos);
	//					Text.Font = GameFont.Tiny;
	//					GUI.color = hediffs[i].Item2.def.defaultLabelColor;
	//					Widgets.Label(new Rect(butRect.x + vector.x, butRect.y + vector.y, butRect.width - 3f, 75f - pos), hediffs[i].Item1.minimumResourcePerUse.ToString());
	//					GUI.color = Color.white;
	//					Text.Font = GameFont.Small;
	//				}
	//			}
	//		}
	//	}
	//
	//	public static bool TryGetAmmoString(Verb verb, out List<Tuple<ResourceProperties, HediffResource>> hediffs)
	//	{
	//		hediffs = new List<Tuple<ResourceProperties, HediffResource>>();
	//		if (verb.CasterIsPawn)
	//		{
	//			var resourceProps = verb.GetResourceProps();
	//			if (resourceProps != null)
    //            {
	//				var resourceSettings = resourceProps.ResourceSettings;
	//				if (resourceSettings != null)
    //                {
	//					foreach (var option in resourceSettings)
	//					{
	//						var resourceHediff = verb.CasterPawn.health.hediffSet.GetFirstHediffOfDef(option.hediff) as HediffResource;
	//						if (resourceHediff != null)
	//						{
	//							hediffs.Add(new Tuple<ResourceProperties, HediffResource>(option, resourceHediff));
	//						}
	//					}
	//				}
    //            }
	//		}
	//		if (hediffs.Count > 0)
    //        {
	//			return true;
    //        }
	//		return false;
	//	}
	//}
}
