using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class IngestionOutcomeDoer_GiveHediffResource : IngestionOutcomeDoer
    {
        public HediffResourceDef hediffDef;

        public float resourceAdjust = 0f;

        public float resourcePercent = -1f;
        public BodyPartDef applyToPart;
        public List<HediffDef> blacklistHediffsPreventAdd;
        public HediffDef blacklistHediffPoison;
        public string blacklistHediffPoisonMessage;
        public string cannotDrinkReason;
        public bool addHediffIfMissing;
        public ChemicalDef toleranceChemical;
        public bool preventFromUsageIfHasBlacklistedHediff;
        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (blacklistHediffsPreventAdd != null)
            {
                foreach (var blacklistHediff in blacklistHediffsPreventAdd)
                {
                    var hd = pawn.health.hediffSet.GetFirstHediffOfDef(blacklistHediff);
                    if (hd != null)
                    {
                        if (blacklistHediffPoison != null)
                        {
                            var poison = HediffMaker.MakeHediff(blacklistHediffPoison, pawn);
                            pawn.health.AddHediff(poison);
                            Messages.Message(blacklistHediffPoisonMessage.Translate(pawn.Named("PAWN"), ingested.Named("INGESTED")), pawn, MessageTypeDefOf.NegativeHealthEvent);
                        }
                        return;
                    }
                }
            }
            HediffResource hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef) as HediffResource;
            if (hediff is null && addHediffIfMissing)
            {
                BodyPartRecord bodyPartRecord = null;
                if (applyToPart != null)
                {
                    bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault((BodyPartRecord x) => x.def == applyToPart);
                }

                hediff = HediffMaker.MakeHediff(hediffDef, pawn, bodyPartRecord) as HediffResource;
                pawn.health.AddHediff(hediff);
            }
            if (hediff != null)
            {
                if (resourceAdjust != 0f)
                {
                    if (toleranceChemical != null)
                    {
                        AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize_NewTemp(pawn, toleranceChemical, ref resourceAdjust, true);
                    }
                    hediff.ChangeResourceAmount(resourceAdjust);
                }
                if (resourcePercent != -1f)
                {
                    var resourceAmount = hediff.ResourceCapacity * resourcePercent;
                    if (toleranceChemical != null)
                    {
                        AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize_NewTemp(pawn, toleranceChemical, ref resourceAmount, true);
                    }
                    hediff.ChangeResourceAmount(resourceAdjust);
                }
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && chance >= 1f)
            {
                foreach (StatDrawEntry item in hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
                {
                    yield return item;
                }
            }
        }
    }
}