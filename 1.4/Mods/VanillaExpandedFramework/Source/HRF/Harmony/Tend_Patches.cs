using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{

    [HarmonyPatch(typeof(HealthAIUtility), "FindBestMedicine")]
    public static class Patch_FindBestMedicine
    {
        private static bool Prefix(Pawn healer, Pawn patient, bool onlyUseInventory = false)
        {
            if (healer.HasEnoughResourceToTend(out _))
            {
                HRFLog.Message("Has enough resource to tend");
                return false;
            }
            return true;
        }

        public static bool HasEnoughResourceToTend(this Pawn healer, out (HediffResource, TendProperties) toConsume)
        {
            if (healer?.health?.hediffSet?.hediffs != null)
            {
                foreach (var hediff in healer.health.hediffSet.hediffs)
                {
                    if (hediff is HediffResource hediffResource && hediffResource.CurStage is HediffStageResource hediffStageResource && hediffStageResource.tendProperties != null)
                    {
                        hediffResource = hediffStageResource.tendProperties.hediffResource != null
                            ? healer.health.hediffSet.GetFirstHediffOfDef(hediffStageResource.tendProperties.hediffResource) as HediffResource
                            : hediffResource;

                        if (hediffResource != null)
                        {
                            bool canTend = hediffResource.ResourceAmount >= hediffStageResource.tendProperties.resourceOnTend;
                            if (canTend)
                            {
                                toConsume = (hediffResource, hediffStageResource.tendProperties);
                                return true;
                            }
                        }
                    }
                }
            }
            toConsume = default;
            return false;
        }
    }

    [HarmonyPatch(typeof(TendUtility), "GetOptimalHediffsToTendWithSingleTreatment")]
    public static class Patch_GetOptimalHediffsToTendWithSingleTreatment
    {
        private static void Prefix(Pawn patient, ref bool usingMedicine, List<Hediff> outHediffsToTend, List<Hediff> tendableHediffsInTendPriorityOrder = null)
        {
            if (TendUtility_DoTend_Patch.healer != null && TendUtility_DoTend_Patch.healer.HasEnoughResourceToTend(out _))
            {
                usingMedicine = true;
            }
        }
    }

    [HarmonyPatch(typeof(TendUtility), "DoTend")]
    public static class TendUtility_DoTend_Patch
    {
        public static Pawn healer;
        private static void Prefix(Pawn doctor)
        {
            healer = doctor;
        }
        private static void Postfix()
        {
            healer = null;
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TendUtility_DoTend_Patch), nameof(DoTendingEffect)));
                }
            }
        }

        public class HardCodedTendData
        {
            public float quality;
            public float maxQuality;
        }

        public static HardCodedTendData hardCodedTendData;
        public static void DoTendingEffect(Pawn doctor, Pawn patient, ref float quality, ref float maxQuality)
        {
            if (hardCodedTendData != null)
            {
                quality = hardCodedTendData.quality;
                maxQuality = hardCodedTendData.maxQuality;
            }
            else if (doctor.HasEnoughResourceToTend(out var data))
            {
                quality = CalculateBaseTendQuality(doctor, patient, data.Item2);
                maxQuality = data.Item2.statBases.First(x => x.stat == StatDefOf.MedicalQualityMax).value;
                data.Item1.ChangeResourceAmount(-data.Item2.resourceOnTend);
            }
        }

        public static float CalculateBaseTendQuality(Pawn doctor, Pawn patient, TendProperties tendEffect)
        {
            float medicinePotency = tendEffect.statBases.First(x => x.stat == StatDefOf.MedicalPotency).value;
            float medicineQualityMax = tendEffect.statBases.First(x => x.stat == StatDefOf.MedicalQualityMax).value;
            return TendUtility.CalculateBaseTendQuality(doctor, patient, medicinePotency, medicineQualityMax);
        }
    }


}