using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class IngestionOutcomeDoer_GiveHediffAdv : IngestionOutcomeDoer
    {
        public List<AdditionalHediffEntry> hediffDefs = new List<AdditionalHediffEntry>();

        public bool randomChosen = false;

        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (randomChosen)
            {
                TryAddHediffFromEntry(pawn, hediffDefs.RandomElementByWeight((AdditionalHediffEntry x) => x.weight));
            }
            else
            {
                foreach (AdditionalHediffEntry entry in hediffDefs)
                {
                    TryAddHediffFromEntry(pawn, entry);
                }
            }
        }

        public void TryAddHediffFromEntry(Pawn pawn, AdditionalHediffEntry entry)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(entry.hediff);
            if (hediff == null)
            {
                Hediff newHediff = HediffMaker.MakeHediff(entry.hediff, pawn);
                newHediff.Severity = entry.severityRange.RandomInRange;
                pawn.health.AddHediff(newHediff);
            }
            else
            {
                if(hediff.Severity < entry.severityRange.min)
                {
                    hediff.Severity = entry.severityRange.RandomInRange;
                }
            }
        }
    }
}
