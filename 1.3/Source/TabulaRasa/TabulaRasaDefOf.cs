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

        public static JobDef TabulaRasa_UseTeleporter;
        public static JobDef TabulaRasa_UseRecall;
        public static JobDef TabulaRasa_Hibernate;
        public static JobDef TabulaRasa_ConsumeEnergySource;
        public static JobDef TabulaRasa_RechargeFromSocket;

        public static HediffDef TabulaRasa_EmergencyPower;
        public static HediffDef TabulaRasa_ProtoBodypart;

        public static StatDef TabulaRasa_EnergyBase;
        public static StatDef TabulaRasa_EnergyMultiplier;
    }
}
