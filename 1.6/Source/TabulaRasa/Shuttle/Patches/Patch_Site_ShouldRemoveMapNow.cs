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
    [HarmonyPatch(typeof(Site), "ShouldRemoveMapNow")]
    public class Patch_Site_ShouldRemoveMapNow
    {
        [HarmonyPrefix]
        public static bool Prefix(Site __instance, ref bool __result)
        {
            if (__instance.Faction != Faction.OfPlayer)
            {
                List<Building> allBuildingsColonist = __instance.Map.listerBuildings.allBuildingsColonist;
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
