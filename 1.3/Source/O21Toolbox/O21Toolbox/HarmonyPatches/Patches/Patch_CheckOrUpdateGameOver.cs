using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.TurretsPlus;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(GameEnder), "CheckOrUpdateGameOver")]
    public static class Patch_CheckOrUpdateGameOver
    {
        [HarmonyPostfix]
        public static void CheckOrUpdateGameOver_Postfix(GameEnder __instance)
        {
            List<Map> maps = Find.Maps;
            foreach(Map map in maps)
            {
                List<Thing> thingList = map.listerThings.ThingsInGroup(ThingRequestGroup.ThingHolder);
                foreach(Thing thing in thingList)
                {
                    if((thing is Building_Bunker || thing is Building_Emplacement) && (bool)((Building_Bunker)thing)?.HasAnyContents)
                    {
                        __instance.gameEnding = false;
                        return;
                    }
                }
            }
        }
    }
}
