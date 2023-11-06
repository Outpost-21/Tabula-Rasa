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
    [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
    public static class Patch_Pawn_HealthTracker_AddHediff
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, Hediff hediff, BodyPartRecord part = null, DamageInfo? dinfo = null, DamageWorker.DamageResult result = null)
        {
            DefModExt_DiseaseImmunity modExt = ___pawn.def.GetModExtension<DefModExt_DiseaseImmunity>();
            if (modExt != null && !modExt.hediffs.NullOrEmpty() && modExt.hediffs.Contains(hediff.def))
            {
                return false;
            }

            HediffComp_AutoHeal autoheal = ___pawn?.health?.hediffSet?.hediffs?.Find(h => h.TryGetComp<HediffComp_AutoHeal>() != null)?.TryGetComp<HediffComp_AutoHeal>();
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

            HediffComp_PassiveHealing passiveHeal = ___pawn?.health?.hediffSet?.hediffs?.Find(h => h.TryGetComp<HediffComp_PassiveHealing>() != null)?.TryGetComp<HediffComp_PassiveHealing>();
            if(passiveHeal != null)
            {
                if (passiveHeal.Props.preventSicknesses)
                {
                    if ((passiveHeal.Props.sicknessWhitelist.NullOrEmpty() && hediff.def.makesSickThought) || passiveHeal.Props.sicknessWhitelist.Contains(hediff.def))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
