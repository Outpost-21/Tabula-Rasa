using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Needs
{
    public static class Utility_ArtificalPawn
    {
        private static List<Hediff> tmpHediffsToTend = new List<Hediff>();
        private static List<Hediff> tmpHediffs = new List<Hediff>();

        public static bool IsArtificial(this Pawn pawn)
        {
            return pawn.def.HasModExtension<ArtificialPawnProperties>();
        }

        public static void DoTend(Pawn doctor, Pawn patient, Thing medicine)
        {
            if (patient.health.HasHediffsNeedingTend(false))
            {
                if (medicine != null && medicine.Destroyed)
                {
                    Log.Warning("Tried to use destroyed repair kit.", false);
                    medicine = null;
                }

                //float quality = 1f; //CalculateBaseTendQuality
                GetOptimalHediffsToTendWithSingleTreatment(patient, medicine != null, tmpHediffsToTend, null);

                for (int i = 0; i < tmpHediffsToTend.Count; i++)
                {
                    //tmpHediffsToTend[i].Tended(quality, i);
                    if (medicine == null)
                    {
                        tmpHediffsToTend[i].Tended_NewTemp(0.1f, i);
                    }
                    else
                    {
                        patient.health.RemoveHediff(tmpHediffsToTend[i]);
                    }
                }
                if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
                {
                    patient.mindState.timesGuestTendedToByPlayer++;
                }
                if (doctor != null && doctor.IsColonistPlayerControlled)
                {
                    patient.records.AccumulateStoryEvent(StoryEventDefOf.TendedByPlayer);
                }
                if (doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal)
                {
                    if (RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f))
                    {
                        if (doctor.Faction != null && doctor.Faction != patient.Faction)
                        {
                            InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, 1f, false);
                        }
                    }
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
                if (medicine != null)
                {
                    if (patient.Spawned || (doctor != null && doctor.Spawned))
                    {
                        if (medicine != null && medicine.GetStatValue(StatDefOf.MedicalPotency, true) > RimWorld.ThingDefOf.MedicineIndustrial.GetStatValueAbstract(StatDefOf.MedicalPotency, null))
                        {
                            SoundDefOf.Building_Complete.PlayOneShot(new TargetInfo(patient.Position, patient.Map, false));
                        }
                    }
                    if (medicine.stackCount > 1)
                    {
                        medicine.stackCount--;
                    }
                    else if (!medicine.Destroyed)
                    {
                        medicine.Destroy(DestroyMode.Vanish);
                    }
                }
            }
        }

        public static void GetOptimalHediffsToTendWithSingleTreatment(Pawn patient, bool usingMedicine, List<Hediff> outHediffsToTend, List<Hediff> tendableHediffsInTendPriorityOrder = null)
        {
            outHediffsToTend.Clear();
            tmpHediffs.Clear();
            if (tendableHediffsInTendPriorityOrder != null)
            {
                tmpHediffs.AddRange(tendableHediffsInTendPriorityOrder);
            }
            else
            {
                List<Hediff> hediffs = patient.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i].TendableNow(false))
                    {
                        tmpHediffs.Add(hediffs[i]);
                    }
                }
                TendUtility.SortByTendPriority(tmpHediffs);
            }
            if (tmpHediffs.Any<Hediff>())
            {
                Hediff hediff = tmpHediffs[0];
                outHediffsToTend.Add(hediff);
                HediffCompProperties_TendDuration hediffCompProperties_TendDuration = hediff.def.CompProps<HediffCompProperties_TendDuration>();
                if (hediffCompProperties_TendDuration != null && hediffCompProperties_TendDuration.tendAllAtOnce)
                {
                    for (int j = 0; j < tmpHediffs.Count; j++)
                    {
                        if (tmpHediffs[j] != hediff && tmpHediffs[j].def == hediff.def)
                        {
                            outHediffsToTend.Add(tmpHediffs[j]);
                        }
                    }
                }
                else if (hediff is Hediff_Injury && usingMedicine)
                {
                    float num = hediff.Severity;
                    for (int k = 0; k < tmpHediffs.Count; k++)
                    {
                        if (tmpHediffs[k] != hediff)
                        {
                            Hediff_Injury hediff_Injury = tmpHediffs[k] as Hediff_Injury;
                            if (hediff_Injury != null)
                            {
                                float severity = hediff_Injury.Severity;
                                if (num + severity <= 20f)
                                {
                                    num += severity;
                                    outHediffsToTend.Add(hediff_Injury);
                                }
                            }
                        }
                    }
                }
                tmpHediffs.Clear();
            }
        }
    }
}
