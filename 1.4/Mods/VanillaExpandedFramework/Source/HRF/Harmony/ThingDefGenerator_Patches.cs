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
	[HarmonyPatch(typeof(ThingDefGenerator_Buildings), "NewFrameDef_Thing")]
	public static class Patch_NewFrameDef_Thing
	{
		private static void Postfix(ref ThingDef __result, ref ThingDef def)
		{
			var options = def.GetModExtension<FacilityInProgress>();
			if (options != null)
            {
				var props = new CompProperties_ThingInUse();
				props.useProperties = options.useProperties;
				__result.comps.Add(props);
			}
		}
	}
}
