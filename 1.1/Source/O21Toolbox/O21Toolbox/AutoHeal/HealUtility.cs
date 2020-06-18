using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Utility;

namespace O21Toolbox.AutoHeal
{
    public static class HealUtility
    {
        public static void SetNextTick(int ticks, int setTicks)
        {
            ticks = Current.Game.tickManager.TicksGame + setTicks;
        }

        public static void TrySealWounds(Pawn pawn, List<HediffDef> ignoredHediffs)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow() && !ignoredHediffs.Contains(hd.def)
                                             select hd;
            bool flag = enumerable != null;
            if (flag)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff != null)
                    {
                        HediffWithComps hediffWithComps = hediff as HediffWithComps;
                        bool flag2 = hediffWithComps != null;
                        if (flag2)
                        {
                            HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                            if(hediffComp_TendDuration != null)
                            {
                                hediffComp_TendDuration.tendQuality = 2f;
                                hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                            }
                            pawn.health.Notify_HediffChanged(hediff);
                        }
                    }
                }
            }
        }

        public static void TryCureInfections(Pawn pawn, List<HediffDef> hediffList, List<HediffDef> explicitRemovals)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.def.makesSickThought || explicitRemovals.Contains(hd.def)
                                             select hd;
            if (enumerable != null)
            {
                List<Hediff> enumerableListed = enumerable.ToList();
                foreach (Hediff hediff in enumerableListed)
                {
                    if(hediff != null)
                    {
                        HediffWithComps hediffWithComps = hediff as HediffWithComps;
                        if (hediffWithComps != null && !hediffList.Any(h => hediffWithComps.def == h))
                        {
                            pawn.health.RemoveHediff(hediff);
                            pawn.health.hediffSet.DirtyCache();
                        }
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
