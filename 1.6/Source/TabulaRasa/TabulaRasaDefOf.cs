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

        public static JobDef 
            TabulaRasa_ReplaceShuttleWeapon,
            TabulaRasa_EnterShuttleAsPilot,
            TabulaRasa_EnterShuttleAsPassenger,
            TabulaRasa_LoadShuttleShell,
            TabulaRasa_InstallShuttleUpgrade,
            TabulaRasa_UninstallShuttleUpgrade,
            TabulaRasa_TakeFromProducer,
            TabulaRasa_UseTeleporter,
            TabulaRasa_UseRecall,
            TabulaRasa_UseEffectApplyHediff,
            TabulaRasa_GatherSlotItem;

        public static HediffDef 
            TabulaRasa_RemovableHediff;

        public static SoundDef 
            EnergyShield_Broken;
    }
}
