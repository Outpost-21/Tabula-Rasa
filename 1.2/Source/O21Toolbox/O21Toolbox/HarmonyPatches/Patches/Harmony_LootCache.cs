using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.LootCache;

namespace O21Toolbox.HarmonyPatches.Patches
{
	[HarmonyPatch(typeof(ThingSetMaker_MapGen_AncientPodContents))]
	[HarmonyPatch("GiveRandomLootInventoryForTombPawn")]
	public class Patch_ThingSetMakerMapGenAncientPodContents_GiveRandomLootInventoryForTombPawn
	{
		[HarmonyPostfix]
		public static void Postfix(Pawn p)
		{
			ThingDef thingDef = null;
			float value = Rand.Value;
			if (value < 0.5f)
			{
				List<LootCacheDef> thingDefList = DefDatabase<LootCacheDef>.AllDefsListForReading;
				thingDef = (ThingDef)thingDefList.RandomElementByWeight(new Func<LootCacheDef, float>(lc => lc.cacheWeight));
			}
			if (thingDef == null)
			{
				return;
			}
			Thing thing = ThingMaker.MakeThing(thingDef, null);
			thing.stackCount = 1;
			p.inventory.innerContainer.TryAdd(thing, true);
		}
	}
}
