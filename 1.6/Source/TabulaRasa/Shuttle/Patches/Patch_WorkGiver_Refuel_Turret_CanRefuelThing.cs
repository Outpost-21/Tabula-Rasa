using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa.Shuttle
{
    [HarmonyPatch(typeof(WorkGiver_Refuel_Turret), "CanRefuelThing")]
    public class Patch_WorkGiver_Refuel_Turret_CanRefuelThing
    {
        [HarmonyPrefix]
        public static bool Prefix(Thing t)
        {
            if (t is Shuttle)
            {
                Shuttle shuttle = t as Shuttle;
                if (!shuttle.Map.areaManager.Home[shuttle.Position])
                {
                    return false;
                }
                else if (!shuttle.IsLanded)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
