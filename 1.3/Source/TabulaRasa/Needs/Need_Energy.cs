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
    public class Need_Energy : Need
    {
        private int lastNonEmergencyTick = -99999;

        public EnergyCategory CurCategory
        {
            get
            {
                if (CurLevelPercentage <= 0f)
                {
                    return EnergyCategory.EmergencyPower;
                }
                if (CurLevelPercentage < (0.3f * 0.4f))
                {
                    return EnergyCategory.Desperate;
                }
                if (CurLevelPercentage < (0.3f * 0.8f))
                {
                    return EnergyCategory.GettingLow;
                }
                return EnergyCategory.Full;
            }
        }

        public bool EmergencyPower => CurCategory == EnergyCategory.EmergencyPower;

        public override int GUIChangeArrow => -1;

        public override bool ShowOnNeedList => !Disabled;

        public bool Disabled
        {
            get
            {
                if (!pawn.Dead && pawn.Spawned)
                {
                    return !pawn.def.HasModExtension<DefModExt_EnergyNeed>();
                }
                return true;
            }
        }

        public float EnergyRate
        {
            get
            {
                float energyRate = pawn.GetStatValue(TabulaRasaDefOf.TabulaRasa_EnergyBase);
                energyRate *= pawn.GetStatValue(TabulaRasaDefOf.TabulaRasa_EnergyMultiplier);
                return energyRate;
            }
        }

        public Need_Energy(Pawn pawn) : base(pawn)
        {

        }

        public override void SetInitialLevel()
        {
            CurLevel = 1f;
        }

        public override void NeedInterval()
        {
            if (Disabled)
            {
                CurLevel = 1f;
                return;
            }
            if (!EmergencyPower)
            {
                lastNonEmergencyTick = Find.TickManager.TicksGame;
            }
            if (!IsFrozen)
            {
                CurLevel -= GetPawnOxygenConsumption() * 1200f;

                if (EmergencyPower)
                {
                    HealthUtility.AdjustSeverity(pawn, TabulaRasaDefOf.TabulaRasa_EmergencyPower, 0.0113333331f * Mathf.Lerp(0.8f, 1.2f, Rand.ValueSeeded(pawn.thingIDNumber ^ 0x26EF7A)));
                }
                else
                {
                    HealthUtility.AdjustSeverity(pawn, TabulaRasaDefOf.TabulaRasa_EmergencyPower, 0f - (0.0113333331f * Mathf.Lerp(0.8f, 1.2f, Rand.ValueSeeded(pawn.thingIDNumber ^ 0x26EF7A))));
                }
            }
        }

        public float GetPawnOxygenConsumption()
        {
            DefModExt_EnergyNeed modExt = pawn.def.GetModExtension<DefModExt_EnergyNeed>();

            if (pawn.InWirelessChargerRange())
            {
                CurLevel = 1f;
                return 0f;
            }
            return 2.66666666E-05f * EnergyRate;
        }
    }
}
