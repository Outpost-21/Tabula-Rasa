using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(MassUtility), "Capacity")]
    public static class Patch_MassUtility_Capacity
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn p, StringBuilder explanation, ref float __result)
		{
			if (p?.apparel?.WornApparel.NullOrEmpty() ?? true)
			{
				return;
			}

			foreach (Apparel item in p.apparel.WornApparel)
			{
				if (item.def is ExtendedApparelDef extApparel)
				{
					__result += extApparel.carryCapBuff;
					explanation?.AppendLine($"{item.LabelCapNoCount}: +{extApparel.carryCapBuff}");
				}
			}
		}
    }
}
