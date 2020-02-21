using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(DaysWorthOfFoodCalculator), "ApproxDaysWorthOfFood", new Type[] {typeof(List<Pawn>), typeof(List<ThingDefCount>), typeof(int), typeof(IgnorePawnsInventoryMode),typeof(Faction), typeof(WorldPath), typeof(float), typeof(int), typeof(bool)})]
    public class Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood
    {
        public static bool Prefix(ref List<Pawn> pawns, List<ThingDefCount> extraFood, int tile, IgnorePawnsInventoryMode ignoreInventory, Faction faction, WorldPath path, float nextTileCostLeft, int caravanTicksPerMove, bool assumeCaravanMoving)
        {
            List<Pawn> modifiedPawnsList = new List<Pawn>(pawns);
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<ArtificialPawnProperties>());
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<DefModExt_SolarNeed>());

            pawns = modifiedPawnsList;
            return true;
        }
    }
}
