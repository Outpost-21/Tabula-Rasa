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
        public static JobDef TabulaRasa_GatherSlotItem;

        public static HediffDef TabulaRasa_RemovableHediff;

        public static SoundDef EnergyShield_Broken;
    }
}
