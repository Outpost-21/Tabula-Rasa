using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Jetpack;

namespace O21Toolbox.HarmonyPatches
{
	[HarmonyPatch(typeof(MapPawns), "AnyPawnBlockingMapRemoval", MethodType.Getter)]
	public static class Patch_MapPawns_AnyPawnBlockingMapRemoval
	{
		[HarmonyPostfix]
		[HarmonyPriority(800)]
		private static void Postfix(ref bool __result, Map ___map)
		{
			if (!__result && GetJetpackSkyfallerOnMap(___map))
			{
				__result = true;
			}
		}

		public static bool GetJetpackSkyfallerOnMap(Map map)
		{
			if (map != null)
			{
				List<Thing> CheckList = map.listerThings.ThingsInGroup(ThingRequestGroup.ThingHolder);
				if (CheckList.Any(t => t is Skyfaller_Jetpack))
				{
					return true;
				}
			}
			return false;
		}
	}
}
