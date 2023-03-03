using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class Verb_Cauterize : BaseVerb
    {
        private static readonly List<Hediff_Injury> injuries = new List<Hediff_Injury>();

        public override bool TryCastShot()
        {
            if (!(currentTarget.Thing is Pawn pawn)) return false;
            injuries.Clear();
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                Log.Message("Found hediff " + hediff + " that is bleeding: " + hediff.Bleeding);
                if (!hediff.Bleeding) continue;
                if (hediff is Hediff_MissingPart mp)
                {
                    mp.IsFresh = false;
                    pawn.health.Notify_HediffChanged(hediff);
                }
                else if (hediff is Hediff_Injury i)
                {
                    injuries.Add(i);
                }
            }

            injuries.ForEach(hediff =>
            {
                pawn.health.RemoveHediff(hediff);
                var hediff2 = HediffMaker.MakeHediff(Props.cauterizeDamageDef.hediff, pawn, hediff.Part);
                hediff2.Severity = hediff.Severity;
                pawn.health.AddHediff(hediff2, hediff.Part);
            });
            injuries.Clear();

            return true;
        }
    }
}