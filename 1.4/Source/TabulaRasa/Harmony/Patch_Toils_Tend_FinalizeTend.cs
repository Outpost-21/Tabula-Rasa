using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Toils_Tend), "FinalizeTend")]
    public class Patch_Toils_Tend_FinalizeTend
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Toil __result, Pawn patient)
        {
            DefModExt_ArtificialPawn modExt = patient.def.GetModExtension<DefModExt_ArtificialPawn>();
            if (modExt != null)
            {
                if (!modExt.canBeRepaired)
                {
                    Toil toil = new Toil();
                    toil.initAction = delegate
                    {
                        return;
                    };
                    toil.defaultCompleteMode = ToilCompleteMode.Instant;
                    __result = toil;
                    return false;
                }
                if (!modExt.repairParts.NullOrEmpty())
                {
                    Toil toil = new Toil();
                    toil.initAction = delegate
                    {
                        Pawn actor = toil.actor;

                        Thing repairParts = actor.CurJob.targetB.Thing;

                        if (!patient.def.GetModExtension<DefModExt_ArtificialPawn>().repairParts.NullOrEmpty() && repairParts != null && !patient.def.GetModExtension<DefModExt_ArtificialPawn>().repairParts.Contains(actor.CurJob.targetB.Thing.def))
                        {
                            repairParts = null;
                        }

                        float num = (!patient.RaceProps.Animal) ? 500f : 175f;
                        float num2 = ThingDefOf.MedicineIndustrial.MedicineTendXpGainFactor;
                        actor.skills.Learn(SkillDefOf.Crafting, num * num2, false);

                        ArtificialUtil.DoTend(actor, patient, repairParts);

                        if (repairParts != null && repairParts.Destroyed)
                        {
                            actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
                        }
                        if (toil.actor.CurJob.endAfterTendedOnce)
                        {
                            actor.jobs.EndCurrentJob(JobCondition.Succeeded, true);
                        }
                    };
                    toil.defaultCompleteMode = ToilCompleteMode.Instant;
                    __result = toil;
                    return false;
                }
            }
            return true;
        }
    }
}
