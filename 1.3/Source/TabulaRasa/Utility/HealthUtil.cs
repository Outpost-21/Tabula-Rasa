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
    public static class HealthUtil
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
            if (enumerable != null)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff != null)
                    {
                        HediffWithComps hediffWithComps = hediff as HediffWithComps;
                        if (hediffWithComps != null)
                        {
                            HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                            if (hediffComp_TendDuration != null)
                            {
                                hediffComp_TendDuration.tendQuality = 2f;
                                hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                            }
                            pawn.health.Notify_HediffChanged(hediff);
                        }
                        Hediff_MissingPart missingBodyPart = hediff as Hediff_MissingPart;
                        if (missingBodyPart != null)
                        {
                            missingBodyPart.Tended(2f, 2f);
                            pawn.health.Notify_HediffChanged(hediff);
                        }
                    }
                }
            }
        }

        public static bool CanSealWounds(Pawn pawn)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow()
                                             select hd;
            if (enumerable != null)
            {
                List<Hediff> list = enumerable.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && !list[i].def.makesSickThought && !list[i].def.chronic)
                    {
                        if (list[i].def.hediffClass == typeof(Hediff_MissingPart))
                        {
                            return false;
                        }
                        else if (!list[i].IsPermanent() && list[i].def.everCurableByItem)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void TrySealWounds(Pawn patient)
        {
            TrySealWounds(patient, new List<HediffDef>());
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
                    if (hediff != null)
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
                        if (hediff2 != null)
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
                using (IEnumerator<BodyPartRecord> enumerator = pawn.GetFirstMatchingBodyparts(pawn.RaceProps.body.corePart,
                                                                                               HediffDefOf.MissingBodyPart,
                                                                                               TabulaRasaDefOf.O21_AutoHeal_ProtoBodypart,
                                                                                               (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        bool flag = hediff2 != null;
                        if (flag)
                        {
                            pawn.health.RemoveHediff(hediff2);
                            pawn.health.AddHediff(TabulaRasaDefOf.O21_AutoHeal_ProtoBodypart, part, null, null);
                            pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
        }

        public static void TendAdditional(Pawn doctor, Pawn patient)
        {
            if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
            {
                patient.mindState.timesGuestTendedToByPlayer++;
            }
            if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
            {
                patient.mindState.timesGuestTendedToByPlayer++;
            }
            if (doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal && patient.RaceProps.playerCanChangeMaster && RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f) && doctor.Faction != null && doctor.Faction != patient.Faction)
            {
                InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, false);
            }
            patient.records.Increment(RecordDefOf.TimesTendedTo);
            if (doctor != null)
            {
                doctor.records.Increment(RecordDefOf.TimesTendedOther);
            }
            if (doctor == patient && !doctor.Dead)
            {
                doctor.mindState.Notify_SelfTended();
            }
            if (ModsConfig.IdeologyActive && doctor != null && doctor.Ideo != null)
            {
                Precept_Role role = doctor.Ideo.GetRole(doctor);
                if (role != null && role.def.roleEffects != null)
                {
                    foreach (RoleEffect roleEffect in role.def.roleEffects)
                    {
                        roleEffect.Notify_Tended(doctor, patient);
                    }
                }
            }
            if (doctor != null && doctor.Faction == Faction.OfPlayer && doctor != patient)
            {
                QuestUtility.SendQuestTargetSignals(patient.questTags, "PlayerTended", patient.Named("SUBJECT"));
            }
        }

        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();
            nextSet.Add(startingPart);
            do
            {
                currentSet.AddRange(nextSet);
                nextSet.Clear();
                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;
                    int num;
                    for (int i = hediffs.Count - 1; i >= 0; i = num - 1)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == part && hediff.def == hediffDef;
                        if (flag)
                        {
                            matchingPart = true;
                            yield return part;
                        }
                        hediff = null;
                        num = i;
                    }
                    bool flag2 = !matchingPart;
                    if (flag2)
                    {
                        for (int j = 0; j < part.parts.Count; j = num + 1)
                        {
                            nextSet.Add(part.parts[j]);
                            num = j;
                        }
                    }
                }
                currentSet.Clear();
            }
            while (nextSet.Count > 0);
            yield break;
        }

        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef hediffExceptionDef)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();
            nextSet.Add(startingPart);
            do
            {
                currentSet.AddRange(nextSet);
                nextSet.Clear();
                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;
                    int num;
                    for (int i = hediffs.Count - 1; i >= 0; i = num - 1)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == part;
                        if (flag)
                        {
                            bool flag2 = hediff.def == hediffExceptionDef;
                            if (flag2)
                            {
                                matchingPart = true;
                                break;
                            }
                            bool flag3 = hediff.def == hediffDef;
                            if (flag3)
                            {
                                matchingPart = true;
                                yield return part;
                                break;
                            }
                        }
                        hediff = null;
                        num = i;
                    }
                    bool flag4 = !matchingPart;
                    if (flag4)
                    {
                        for (int j = 0; j < part.parts.Count; j = num + 1)
                        {
                            nextSet.Add(part.parts[j]);
                            num = j;
                        }
                    }
                }
                currentSet.Clear();
            }
            while (nextSet.Count > 0);
            yield break;
        }

        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef hediffExceptionDef, Predicate<Hediff> extraExceptionPredicate)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();
            nextSet.Add(startingPart);
            do
            {
                currentSet.AddRange(nextSet);
                nextSet.Clear();
                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;
                    int num;
                    for (int i = hediffs.Count - 1; i >= 0; i = num - 1)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == part;
                        if (flag)
                        {
                            bool flag2 = hediff.def == hediffExceptionDef || extraExceptionPredicate(hediff);
                            if (flag2)
                            {
                                matchingPart = true;
                                break;
                            }
                            bool flag3 = hediff.def == hediffDef;
                            if (flag3)
                            {
                                matchingPart = true;
                                yield return part;
                                break;
                            }
                        }
                        hediff = null;
                        num = i;
                    }
                    bool flag4 = !matchingPart;
                    if (flag4)
                    {
                        for (int j = 0; j < part.parts.Count; j = num + 1)
                        {
                            nextSet.Add(part.parts[j]);
                            num = j;
                        }
                    }
                }
                currentSet.Clear();
            }
            while (nextSet.Count > 0);
            yield break;
        }

        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef[] hediffExceptionDefs)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();
            nextSet.Add(startingPart);
            do
            {
                currentSet.AddRange(nextSet);
                nextSet.Clear();
                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;
                    int num;
                    for (int i = hediffs.Count - 1; i >= 0; i = num - 1)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == part;
                        if (flag)
                        {
                            bool flag2 = hediffExceptionDefs.Contains(hediff.def);
                            if (flag2)
                            {
                                matchingPart = true;
                                break;
                            }
                            bool flag3 = hediff.def == hediffDef;
                            if (flag3)
                            {
                                matchingPart = true;
                                yield return part;
                                break;
                            }
                        }
                        hediff = null;
                        num = i;
                    }
                    bool flag4 = !matchingPart;
                    if (flag4)
                    {
                        for (int j = 0; j < part.parts.Count; j = num + 1)
                        {
                            nextSet.Add(part.parts[j]);
                            num = j;
                        }
                    }
                }
                currentSet.Clear();
            }
            while (nextSet.Count > 0);
            yield break;
        }

        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef[] hediffDefs)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();
            nextSet.Add(startingPart);
            do
            {
                currentSet.AddRange(nextSet);
                nextSet.Clear();
                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;
                    int num;
                    for (int i = hediffs.Count - 1; i >= 0; i = num - 1)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == part && hediffDefs.Contains(hediff.def);
                        if (flag)
                        {
                            matchingPart = true;
                            yield return part;
                            break;
                        }
                        hediff = null;
                        num = i;
                    }
                    bool flag2 = !matchingPart;
                    if (flag2)
                    {
                        for (int j = 0; j < part.parts.Count; j = num + 1)
                        {
                            nextSet.Add(part.parts[j]);
                            num = j;
                        }
                    }
                }
                currentSet.Clear();
            }
            while (nextSet.Count > 0);
            yield break;
        }

        public static void ReplaceHediffFromBodypart(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef replaceWithDef)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> list = new List<BodyPartRecord>();
            List<BodyPartRecord> list2 = new List<BodyPartRecord>();
            list2.Add(startingPart);
            do
            {
                list.AddRange(list2);
                list2.Clear();
                foreach (BodyPartRecord bodyPartRecord in list)
                {
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        Hediff hediff = hediffs[i];
                        bool flag = hediff.Part == bodyPartRecord && hediff.def == hediffDef;
                        if (flag)
                        {
                            Hediff hediff2 = hediffs[i];
                            hediffs.RemoveAt(i);
                            hediff2.PostRemoved();
                            Hediff item = HediffMaker.MakeHediff(replaceWithDef, pawn, bodyPartRecord);
                            hediffs.Insert(i, item);
                        }
                    }
                    for (int j = 0; j < bodyPartRecord.parts.Count; j++)
                    {
                        list2.Add(bodyPartRecord.parts[j]);
                    }
                }
                list.Clear();
            }
            while (list2.Count > 0);
        }
    }
}
