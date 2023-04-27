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
    [HarmonyPatch(typeof(Pawn_GeneTracker), "OverrideAllConflicting")]
    public static class Patch_Pawn_GeneTracker_OverrideAllConflicting
    {
        [HarmonyPrefix]
        public static bool PreFix(Pawn_GeneTracker __instance, Pawn ___pawn, Gene gene)
        {
            if(gene as Gene_MaleOnly != null)
            {
                if (___pawn.gender != Gender.Male)
                {
                    return false;
                }
                OverrideAllConflicting(___pawn.gender);
            }
            if(gene as Gene_FemaleOnly != null)
            {
                if(___pawn.gender != Gender.Female)
                {
                    return false;
                }
                OverrideAllConflicting(___pawn.gender);
            }
            return true;
            void OverrideAllConflicting(Gender gen)
            {
                gene.OverrideBy(null);
                foreach (Gene item in __instance.GenesListForReading)
                {
                    if (item != gene && item.def.ConflictsWith(gene.def))
                    {
                        item.OverrideBy(gene);
                    }
                }
            }
        }
	}
}
