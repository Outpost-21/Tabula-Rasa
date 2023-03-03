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
	[HarmonyPatch(typeof(Hediff), "PostRemoved")]
	public static class Patch_PostRemoved
	{
		private static void Postfix(Hediff __instance)
		{
			var pawn = __instance?.pawn;
			if (pawn != null)
			{
				foreach (var hediffDef in DefDatabase<HediffResourceDef>.AllDefs)
				{
					if (hediffDef.requiredHediffs != null)
					{
						foreach (var requiredHediff in hediffDef.requiredHediffs)
						{
							while (requiredHediff.minCount > pawn.health.hediffSet.hediffs.Where(x => x.def == requiredHediff.hediff).Count())
							{
								var oldHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
								if (oldHediff != null)
								{
									pawn.health.RemoveHediff(oldHediff);
								}
								else
								{
									break;
								}
							}
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Hediff), "PostAdd")]
	public static class Patch_PostAdd
	{
		private static void Postfix(Hediff __instance)
		{
            var pawn = __instance?.pawn;
			if (pawn != null)
			{
				Utils.hediffResourcesCache.Remove(pawn);
				foreach (var hediffDef in DefDatabase<HediffResourceDef>.AllDefs)
                {
					if (hediffDef.requiredHediffs != null && pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef) is null)
                    {
						bool canAddHediff = true;
						foreach (var requiredHediff in hediffDef.requiredHediffs)
                        {
							var hediffs = pawn.health.hediffSet.hediffs.Where(x => x.def == requiredHediff.hediff);
							if (hediffs.Count() < requiredHediff.minCount || hediffs.Where(x => x.Severity >= requiredHediff.minSeverity).Count() != hediffs.Count())
							{
								canAddHediff = false;
							}
						}

						if (canAddHediff)
                        {
							var newHediff = HediffMaker.MakeHediff(hediffDef, pawn);
							pawn.health.AddHediff(newHediff);
						}
                    }
                }

				var apparels = pawn.apparel?.WornApparel?.ToList();
				if (apparels != null)
				{
					foreach (var apparel in apparels)
					{
						var hediffComp = apparel.GetComp<CompApparelAdjustHediffs>();
						if (hediffComp?.Props.resourceSettings != null)
						{
							foreach (var option in hediffComp.Props.resourceSettings)
							{
								if (option.dropWeaponOrApparelIfBlacklistHediff?.Contains(__instance.def) ?? false)
								{
									pawn.apparel.TryDrop(apparel);
								}
							}
						}
					}
				}

				var equipments = pawn?.equipment?.AllEquipmentListForReading;
				if (equipments != null)
				{
					foreach (var equipment in equipments)
					{
						var hediffComp = equipment.GetComp<CompWeaponAdjustHediffs>();
						if (hediffComp?.Props.resourceSettings != null)
						{
							foreach (var option in hediffComp.Props.resourceSettings)
							{
								if (option.dropWeaponOrApparelIfBlacklistHediff?.Contains(__instance.def) ?? false)
								{
									pawn.equipment.TryDropEquipment(equipment, out ThingWithComps result, pawn.Position);
								}
							}
						}
					}
				}
			}
		}
	}
}
