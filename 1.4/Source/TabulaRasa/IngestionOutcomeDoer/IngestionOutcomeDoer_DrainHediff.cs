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
    public class IngestionOutcomeDoer_DrainHediff : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public float severity = 1.0f;

        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if(hediffDef != null)
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if(hediff != null)
                {
                    hediff.Severity -= severity;
                }
            }
        }
    }
}
