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
    [HarmonyPatch(typeof(Pawn_TrainingTracker), "TrainingTrackerTickRare")]
    public static class Patch_Pawn_TrainingTracker_TrainingTrackerTickRare
    {
        [HarmonyPrefix]
        public static bool PreFix(ref Pawn ___pawn)
        {
            DefModExt_RaceProperties raceProps = ___pawn.def.GetModExtension<DefModExt_RaceProperties>();
            if(raceProps != null && !raceProps.trainingDecays)
            {
                return false;
            }
            return true;
        }
    }
}
