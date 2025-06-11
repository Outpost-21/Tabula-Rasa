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
    [HarmonyPatch(typeof(WorkGiver_Warden_Feed), "JobOnThing")]
    public static class Patch_WardenFeed_JobOnThing
    {
        [HarmonyPrefix]
        public static bool Prefix(ref WorkGiver_Warden_Feed __instance, Pawn pawn, Thing t, bool forced)
        {
            if (t is Pawn prisoner && !prisoner.def.race.EatsFood)
            {
                return false;
            }
            return true;
        }
    }
}
