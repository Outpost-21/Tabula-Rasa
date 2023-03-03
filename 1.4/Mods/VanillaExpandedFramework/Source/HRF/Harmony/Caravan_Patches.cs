using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace HRF
{
	[HarmonyPatch(new Type[]
	{
		typeof(List<Pawn>),
		typeof(float),
		typeof(float),
		typeof(StringBuilder)
	})]
	[HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove")]
	public static class GetTicksPerMove_Patch
	{
		public static List<Pawn> pawnsStatic;
		private static void Postfix(List<Pawn> pawns, ref int __result, float massUsage, float massCapacity, StringBuilder explanation = null)
		{
			if (pawns.Any())
            {
				pawnsStatic = pawns;
				var averageStatValue = pawns.Select(x => x.GetStatValue(HRFDefOf.HFR_CaravanSpeedFactor)).Average();
				__result = (int)(__result / averageStatValue);
			}
		}
	}


	[HarmonyPatch(typeof(WorldPathGrid), "HillinessMovementDifficultyOffset")]
	public static class HillinessMovementDifficultyOffset_Patch
	{
		private static void Postfix(ref float __result)
		{
			if (GetTicksPerMove_Patch.pawnsStatic != null)
            {
				var pawns = GetTicksPerMove_Patch.pawnsStatic;
				if (pawns.Any())
				{
					var averageStatValue = pawns.Select(x => x.GetStatValue(HRFDefOf.HFR_CaravanDifficultyFactor)).Average();
					__result = __result * averageStatValue;
				}
			}
		}
	}

	[HarmonyPatch(typeof(WorldPathGrid), "CalculatedMovementDifficultyAt")]
	public static class CalculatedMovementDifficultyAt_Patch
	{
		private static void Prefix(out float __state, int tile)
		{
			Tile tile2 = Find.WorldGrid[tile];
			__state = tile2.biome.movementDifficulty;
			if (GetTicksPerMove_Patch.pawnsStatic != null)
			{
				var pawns = GetTicksPerMove_Patch.pawnsStatic;
				if (pawns.Any())
				{
					var averageStatValue = pawns.Select(x => x.GetStatValue(HRFDefOf.HFR_CaravanDifficultyFactor)).Average();
					tile2.biome.movementDifficulty = tile2.biome.movementDifficulty * averageStatValue;
				}
			}
		}
		private static void Postfix(float __state, int tile)
		{
			Tile tile2 = Find.WorldGrid[tile];
			tile2.biome.movementDifficulty = __state;
		}
	}

	[HarmonyPatch(typeof(WorldPathGrid), "GetCurrentWinterMovementDifficultyOffset")]
	public static class GetCurrentWinterMovementDifficultyOffset_Patch
	{
		private static void Postfix(ref float __result)
		{
			if (GetTicksPerMove_Patch.pawnsStatic != null)
			{
				var pawns = GetTicksPerMove_Patch.pawnsStatic;
				if (pawns.Any())
				{
					var averageStatValue = pawns.Select(x => x.GetStatValue(HRFDefOf.HFR_CaravanDifficultyFactor)).Average();
					__result = __result * averageStatValue;
				}
				GetTicksPerMove_Patch.pawnsStatic = null;
			}
		}
	}
}
