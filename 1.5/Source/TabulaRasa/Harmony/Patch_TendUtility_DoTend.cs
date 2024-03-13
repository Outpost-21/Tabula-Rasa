using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(TendUtility), "DoTend")]
    static class Patch_TendUtility_DoTend
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn doctor, Pawn patient, Medicine medicine)
        {
            if (medicine != null)
            {
                Comp_UseHealthPack healthPackComp = medicine.TryGetComp<Comp_UseHealthPack>();
                if (healthPackComp != null)
                {
                    HealthUtil.TrySealWounds(patient, new List<HediffDef>());
                    HealthUtil.TendAdditional(doctor, patient);
                    if (medicine != null)
                    {
                        if (medicine.stackCount > 1)
                        {
                            medicine.stackCount--;
                        }
                        if (!medicine.Destroyed)
                        {
                            medicine.Destroy(DestroyMode.Vanish);
                        }
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
