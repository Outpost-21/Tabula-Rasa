using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa.Shuttle
{
    [HarmonyPatch(typeof(SettlementDefeatUtility), "IsDefeated")]
    public class Patch_SettlementDefeatUtility_IsDefeated
    {
        [HarmonyPrefix]
        public static bool Prefix(Map map, ref bool __result)
        {
            if (map.ParentFaction != Faction.OfPlayer)
            {
                List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
                foreach (Building item in allBuildingsColonist)
                {
                    if (item is Shuttle)
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
