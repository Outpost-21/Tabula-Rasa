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
    public class HediffComp_PassiveHealing : HediffComp
    {
        public HediffCompProperties_PassiveHealing Props => (HediffCompProperties_PassiveHealing)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (Find.TickManager.TicksAbs % Props.healTicks == 0)
            {
                if (Props.healWounds)
                {
                    TryHealWounds();
                }
            }
            if (Find.TickManager.TicksAbs % Props.sickTicks == 0)
            {
                if (Props.healSickness)
                {
                    TryHealSickness();
                }
            }
            if (Find.TickManager.TicksAbs % Props.regrowTicks == 0)
            {
                if (Props.regrowParts)
                {
                    TryRegrowParts();
                }
            }
        }

        public void TryHealWounds()
        {
            IEnumerable<Hediff> healableHediffs = GetHealableHediffs();

            if (!healableHediffs.EnumerableNullOrEmpty())
            {
                foreach (Hediff hediff in healableHediffs)
                {
                    hediff.Heal(Props.healWoundsVal);
                    if(hediff.TendableNow() && Props.tendWounds)
                    {
                        hediff.Tended(Props.tendQuality, Props.tendQuality);
                    }
                    if (Props.healWoundsSeq)
                    {
                        return;
                    }
                }
            }

            IEnumerable<Hediff> GetHealableHediffs()
            {
                List<Hediff> allHediffs = Pawn.health.hediffSet.hediffs;
                if (!allHediffs.NullOrEmpty())
                {
                    foreach (Hediff hediff in allHediffs)
                    {
                        if(hediff is Hediff_Injury && !Props.woundBlacklist.Contains(hediff.def))
                        {
                            yield return hediff;
                        }
                    }
                }
                yield break;
            }
        }

        public void TryHealSickness()
        {
            IEnumerable<Hediff> sicknessHediffs = GetSicknessHediffs();

            if (!sicknessHediffs.EnumerableNullOrEmpty())
            {
                foreach (Hediff hediff in sicknessHediffs)
                {
                    hediff.Heal(Props.healSicknessVal);
                    if (Props.healSicknessSeq)
                    {
                        return;
                    }
                }
            }

            IEnumerable<Hediff> GetSicknessHediffs()
            {
                List<Hediff> allHediffs = Pawn.health.hediffSet.hediffs;
                if (!allHediffs.NullOrEmpty())
                {
                    foreach (Hediff hediff in allHediffs)
                    {
                        if ((Props.sicknessWhitelist.NullOrEmpty() ? hediff.def.makesSickThought : false || Props.sicknessWhitelist.Contains(hediff.def)) && !Props.sicknessBlacklist.Contains(hediff.def))
                        {
                            yield return hediff;
                        }
                    }
                }
                yield break;
            }
        }

        public void TryRegrowParts()
        {
            if (Props.regrowingPartDef != null)
            {
                using (IEnumerator<BodyPartRecord> enumerator = Pawn.GetFirstMatchingBodyparts(Pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, Props.regrowingPartDef, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = Pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        if (hediff2 != null)
                        {
                            Pawn.health.RemoveHediff(hediff2);
                            Pawn.health.AddHediff(Props.regrowingPartDef, part, null, null);
                            Pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
            else
            {
                LogUtil.Error($"Hediff {Def.defName} set to regrow parts but has no HediffDef for the regrowablePart to actually do so.");
            }
        }
    }
}
