using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using Harmony;

using O21Toolbox.Utility;

namespace O21Toolbox.AutoHeal
{
    public static class HealUtility
    {
        public static void SetNextHealTick(int ticks, int healTicks)
        {
            ticks = Current.Game.tickManager.TicksGame + healTicks;
        }

        public static void SetNextGrowTick(int ticks, int growthTicks)
        {
            ticks = Current.Game.tickManager.TicksGame + growthTicks;
        }

        public static void TrySealWounds(Pawn pawn)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow()
                                             select hd;
            bool flag = enumerable != null;
            if (flag)
            {
                foreach (Hediff hediff in enumerable)
                {
                    HediffWithComps hediffWithComps = hediff as HediffWithComps;
                    bool flag2 = hediffWithComps != null;
                    if (flag2)
                    {
                        HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                        hediffComp_TendDuration.tendQuality = 2f;
                        hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        pawn.health.Notify_HediffChanged(hediff);
                    }
                }
            }
        }

        public static void TryRegrowBodyparts(Pawn pawn, HediffDef protoBodyPart)
        {
            if (protoBodyPart != null)
            {
                using (IEnumerator<BodyPartRecord> enumerator = pawn.GetFirstMatchingBodyparts(pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, protoBodyPart, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        bool flag = hediff2 != null;
                        if (flag)
                        {
                            pawn.health.RemoveHediff(hediff2);
                            pawn.health.AddHediff(protoBodyPart, part, null, null);
                            pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
            else
            {
                using (IEnumerator<BodyPartRecord> enumerator = pawn.GetFirstMatchingBodyparts(pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, HediffDefOf_AutoHeal.AutoHeal_ProtoBodypart, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        bool flag = hediff2 != null;
                        if (flag)
                        {
                            pawn.health.RemoveHediff(hediff2);
                            pawn.health.AddHediff(HediffDefOf_AutoHeal.AutoHeal_ProtoBodypart, part, null, null);
                            pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
        }
    }
}
