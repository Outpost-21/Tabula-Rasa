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
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
    public class Patch_PawnDiedOrDownedThoughtsUtility_AppendThoughts_ForHumanlike
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Pawn victim)
        {
            DefModExt_ArtificialPawn modExtVictim = victim.def.GetModExtension<DefModExt_ArtificialPawn>();
            if (modExtVictim != null && !modExtVictim.deathMatters)
            {
                return false;
            }
            return true;
        }

    }
}
