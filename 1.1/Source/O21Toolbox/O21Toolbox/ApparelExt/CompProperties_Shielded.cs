using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class CompProperties_Shielded : CompProperties
    {
        public CompProperties_Shielded()
        {
            this.compClass = typeof(Comp_Shielded);
        }

        public bool protectMelee = false;
        public bool protectRanged = true;
        public bool protectExplosive = false;
        //public bool protectPsychic = false;

        public bool empVulnerable = true;

        public int ticksToReset = 3200;

        public float energyOnReset = 0.2f;

        public float energyLossPerDamage = 0.033f;

        public float rechargeRate = 0.13f;

        public float energyMax = 1.1f;

        public string texturePath = "Other/ShieldBubble";

        public SoundDef breakSound = SoundDefOf.EnergyShield_Broken;

        public SoundDef impactSound = SoundDefOf.EnergyShield_AbsorbDamage;

        public SoundDef resetSound = SoundDefOf.EnergyShield_Reset;
    }
}
