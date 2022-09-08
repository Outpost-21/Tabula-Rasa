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
        
        // Shift to standalone mod for intelligent trainable animals?
        //public static TrainabilityDef TabulaRasa_Humanlike;
        //public static TrainableDef TabulaRasa_Clean;
        //public static TrainableDef TabulaRasa_Construction;
        //public static TrainableDef TabulaRasa_Cooking;
        //public static TrainableDef TabulaRasa_Crafting;
        //public static TrainableDef TabulaRasa_Firefighter;
        //public static TrainableDef TabulaRasa_Haul;
        //public static TrainableDef TabulaRasa_Mining;
        //public static TrainableDef TabulaRasa_Plants;
        //public static TrainableDef TabulaRasa_Research;
        //public static WorkTypeDef Cleaning;
        //public static WorkTypeDef Cooking;

        public static NeedDef TabulaRasa_EnergyNeed;

        public static JobDef TabulaRasa_TakeFromProducer;
        public static JobDef TabulaRasa_UseTeleporter;
        public static JobDef TabulaRasa_UseRecall;
        public static JobDef TabulaRasa_Hibernate;
        public static JobDef TabulaRasa_ConsumeEnergySource;
        public static JobDef TabulaRasa_RechargeFromSocket;

        public static HediffDef TabulaRasa_EmergencyPower;
        public static HediffDef TabulaRasa_RemovableHediff;

        public static StatDef TabulaRasa_EnergyMultiplier;

        public static FleshTypeDef TabulaRasa_Artificial;
    }
}
