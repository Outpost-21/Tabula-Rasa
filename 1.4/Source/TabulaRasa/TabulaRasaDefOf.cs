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
    [DefOf]
    public static class TabulaRasaDefOf
    {
        static TabulaRasaDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TabulaRasaDefOf));
        }

        public static JobDef TabulaRasa_TakeFromProducer;
        public static JobDef TabulaRasa_UseTeleporter;
        public static JobDef TabulaRasa_UseRecall;
        public static JobDef TabulaRasa_UseEffectApplyHediff;

        public static HediffDef TabulaRasa_RemovableHediff;

        /// <summary>
        /// To Be Removed
        /// Now shifted to the standalone Asimov. Cannot fully remove till next major update.
        /// </summary>
        public static NeedDef TabulaRasa_EnergyNeed;
        public static JobDef TabulaRasa_Hibernate;
        public static JobDef TabulaRasa_ConsumeEnergySource;
        public static JobDef TabulaRasa_RechargeFromSocket;
        public static HediffDef TabulaRasa_EmergencyPower;
        public static StatDef TabulaRasa_EnergyMultiplier;
        public static FleshTypeDef TabulaRasa_Artificial;
    }
}
