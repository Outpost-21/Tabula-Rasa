using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Shield
{
    public class CompProperties_Shield : CompProperties
    {
        // Shield Generator
        public bool interceptAirProjectiles = false;
        public bool interceptGroundProjectiles = false;
        public bool interceptNonHostileProjectiles = true;
        public bool interceptOutgoingProjectiles = false;

        public EffecterDef reactivateEffect = EffecterDefOf.ActivatorProximityTriggered;

        public string stressLabel = "Shield Stress Level";

        public int resetTime = 30000;

        public SoundDef startupSound = SoundDefOf.Power_OnSmall;
        public SoundDef shutdownSound = SoundDefOf.Power_OffSmall;
        public SoundDef impactSound = SoundDefOf.EnergyShield_AbsorbDamage;
        public SoundDef breakSound = SoundDefOf.EnergyShield_Broken;

        public FloatRange powerUsageRange = new FloatRange(0f, 0f);

        public bool useAmbientCooling = false;
        public float maximumHeatLevel = 0f;
        public FloatRange heatGenRange = new FloatRange(0f, 100f);
        public float stressReduction = 1f;

        public float stressPerDamage = 0.003f;
        public float empDamageFactor = 5.0f;
        public float shieldOverloadThreshold = 0.9f;
        public float shieldOverloadChance = 0.3f;
        public int extraOverloadRange = 3;
        public DamageDef overloadDamageType = DamageDefOf.EMP;

        public bool explodeOnCollapse = false;

        public bool shieldCanBeOffset = false;
        public bool shieldCanBeScaled = false;
        public IntRange shieldScaleLimits = new IntRange(0, 10);
        public int shieldScaleDefault = 5;

        public bool shieldCanBeColored = true;
        public Color shieldColour = Color.white;
        public bool drawInterceptCone;
        public float minAlpha;
        public float idlePulseSpeed;

        public bool podBlocker = true;
    }
}
