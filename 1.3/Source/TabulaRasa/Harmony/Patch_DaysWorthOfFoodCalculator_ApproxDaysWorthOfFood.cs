using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using RimWorld.Planet;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(DaysWorthOfFoodCalculator), "ApproxDaysWorthOfFood", new Type[] { typeof(List<Pawn>), typeof(List<ThingDefCount>), typeof(int), typeof(IgnorePawnsInventoryMode), typeof(Faction), typeof(WorldPath), typeof(float), typeof(int), typeof(bool) })]
    public static class Patch_DaysWorthOfFoodCalculator_ApproxDaysWorthOfFood
    {
        [HarmonyPrefix]
        public static bool Prefix(ref List<Pawn> pawns, List<ThingDefCount> extraFood, int tile, IgnorePawnsInventoryMode ignoreInventory, Faction faction, WorldPath path, float nextTileCostLeft, int caravanTicksPerMove, bool assumeCaravanMoving)
        {
            List<Pawn> list = new List<Pawn>(pawns);
            list.RemoveAll((Pawn pawn) => pawn.RaceProps.EatsFood);
            pawns = list;
            return true;
        }
    }
}
