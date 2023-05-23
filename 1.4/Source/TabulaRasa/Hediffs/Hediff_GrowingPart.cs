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
    public class Hediff_GrowingPart : Hediff_AddedPart
    {

        public override bool ShouldRemove
        {
            get
            {
                return this.Severity >= this.def.maxSeverity;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.TipStringExtra);
                stringBuilder.AppendLine(this.def.GetModExtension<DefModExt_AutoHealProps>().growthText + GenText.ToStringPercent(this.Severity));
                return stringBuilder.ToString();
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if(Severity >= 1f)
            {
                HediffDef hediffDef = pawn?.health?.hediffSet?.GetFirstHediffOfDef(def.GetModExtension<DefModExt_AutoHealProps>().autoHealHediff, false)?.def ?? null;
                if(hediffDef == null)
                {
                    pawn.ReplaceHediffFromBodypart(Part, HediffDefOf.MissingBodyPart, TabulaRasaDefOf.TabulaRasa_RemovableHediff);
                }
                DefModExt_AutoHealProps modExt = hediffDef.GetModExtension<DefModExt_AutoHealProps>();
                if (modExt != null && modExt.curedBodyPart != null)
                {
                    pawn.ReplaceHediffFromBodypart(Part, HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetFirstHediffOfDef(def.GetModExtension<DefModExt_AutoHealProps>().autoHealHediff, false).def.GetModExtension<DefModExt_AutoHealProps>().curedBodyPart);
                }
                else
                {
                    pawn.ReplaceHediffFromBodypart(Part, HediffDefOf.MissingBodyPart, TabulaRasaDefOf.TabulaRasa_RemovableHediff);
                }
            }
        }
    }
}
