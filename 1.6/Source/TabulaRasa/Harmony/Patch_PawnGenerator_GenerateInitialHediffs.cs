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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateInitialHediffs")]
    public static class Patch_PawnGen_GenerateInitialHediffs
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            DefModExt_PawnKindExtended modExt = pawn.kindDef.GetModExtension<DefModExt_PawnKindExtended>();

            if (modExt != null && !pawn.Dead)
            {
                if (!modExt.additionalHediffs.NullOrEmpty())
                {
                    if (modExt.randomAdditionalHediff)
                    {
                        AdditionalHediffEntry result = GetHediffEntry(pawn.kindDef);
                        if (result != null)
                        {
                            GiveHediffEntry(result, pawn);
                        }
                    }
                    else
                    {
                        foreach (AdditionalHediffEntry entry in modExt.additionalHediffs)
                        {
                            GiveHediffEntry(entry, pawn);
                        }
                    }
                }

                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

                if (modExt.clearChronicIllness)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (hediffs[i].def.chronic)
                        {
                            pawn.health.RemoveHediff(hediffs[i]);
                        }
                    }
                }


                if (modExt.clearAddictions)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (hediffs[i].def.IsAddiction)
                        {
                            pawn.health.RemoveHediff(hediffs[i]);
                        }
                    }
                }

                if (modExt.replaceMissingParts)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (pawn.health.hediffSet.PartIsMissing(hediffs[i].part))
                        {
                            pawn.health.RestorePart(hediffs[i].part);
                        }
                    }
                }
            }
        }

        public static void GiveHediffEntry(AdditionalHediffEntry entry, Pawn pawn)
        {
            Hediff hediff = HediffMaker.MakeHediff(entry.hediff, pawn);
            hediff.Severity = entry.severityRange.RandomInRange;
            pawn.health.AddHediff(hediff);
        }

        public static AdditionalHediffEntry GetHediffEntry(PawnKindDef pawnkind)
        {
            if (pawnkind.HasModExtension<DefModExt_PawnKindExtended>())
            {
                DefModExt_PawnKindExtended modExt = pawnkind.GetModExtension<DefModExt_PawnKindExtended>();
                AdditionalHediffEntry resultEntry;
                if (!modExt.additionalHediffs.NullOrEmpty())
                {
                    Func<AdditionalHediffEntry, float> selector = (AdditionalHediffEntry x) => x.weight;
                    resultEntry = modExt.additionalHediffs.RandomElementByWeight(selector);

                    return resultEntry;
                }
            }

            return null;
        }
    }
}
