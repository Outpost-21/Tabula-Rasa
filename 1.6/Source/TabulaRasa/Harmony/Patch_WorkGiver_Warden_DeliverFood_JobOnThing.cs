using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "JobOnThing")]
    public static class Patch_WardenDeliverFood_JobOnThing
    {
        [HarmonyPrefix]
        public static bool Prefix(ref WorkGiver_Warden_DeliverFood __instance, Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (t is Pawn prisoner && !prisoner.def.race.EatsFood)
            {
                return false;
            }
            return true;
        }
    }
}
