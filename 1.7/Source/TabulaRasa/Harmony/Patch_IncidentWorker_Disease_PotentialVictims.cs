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
    [HarmonyPatch(typeof(IncidentWorker_Disease), "PotentialVictims")]
    public static class Patch_IncidentWorker_Disease_PotentialVictims
    {
        [HarmonyPostfix]
        public static void PostFix(ref IEnumerable<Pawn> __result)
        {
            List<Pawn> pawns = new List<Pawn>();
            foreach(Pawn pawn in __result)
            {
                DefModExt_RaceProperties raceProps = pawn.def.GetModExtension<DefModExt_RaceProperties>();
                if(raceProps != null && !raceProps.diseasesEnabled)
                {
                    pawns.Add(pawn);
                }
            }
            if (!pawns.NullOrEmpty())
            {
                List<Pawn> results = __result.ToList();
                foreach(Pawn pawn in pawns)
                {
                    results.Remove(pawn);
                }
                __result = results;
            }
        }
    }
}
