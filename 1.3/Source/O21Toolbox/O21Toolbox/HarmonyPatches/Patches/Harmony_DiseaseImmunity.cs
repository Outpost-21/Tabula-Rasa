using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.AutoHeal;
using O21Toolbox.PawnExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_DiseaseImmunity
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[]
        {
            typeof(Hediff),
            typeof(BodyPartRecord),
            typeof(DamageInfo?),
            typeof(DamageWorker.DamageResult) 
        })]
        public static class Patch_Pawn_HealthTracker_AddHediff
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, Hediff hediff, BodyPartRecord part = null, DamageInfo? dinfo = null, DamageWorker.DamageResult result = null)
            {
                DefModExt_DiseaseImmunity modExt = ___pawn.def.GetModExtension<DefModExt_DiseaseImmunity>();
                if(modExt != null && !modExt.hediffs.NullOrEmpty() && modExt.hediffs.Contains(hediff.def))
                {
                    return false;
                }

                Comp_AutoHeal autoheal = ___pawn?.health?.hediffSet?.hediffs?.Find(h => h.TryGetComp<Comp_AutoHeal>() != null)?.TryGetComp<Comp_AutoHeal>();
                if (autoheal != null)
                {
                    if (autoheal.Props.removeInfections && hediff.def.makesSickThought)
                    {
                        return false;
                    }
                    if (!autoheal.Props.explicitRemovals.NullOrEmpty() && autoheal.Props.explicitRemovals.Contains(hediff.def))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
