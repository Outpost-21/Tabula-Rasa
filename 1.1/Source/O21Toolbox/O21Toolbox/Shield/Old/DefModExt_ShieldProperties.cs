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
    public class DefModExt_ShieldProperties : DefModExtension
    {
        public static readonly DefModExt_ShieldProperties defaultValues = new DefModExt_ShieldProperties();

        public int resetTime = 30000;

        public SoundDef startupSound = SoundDefOf.Power_OnSmall;
        public SoundDef shutdownSound = SoundDefOf.Power_OffSmall;
        public SoundDef impactSound = SoundDefOf.PsycastPsychicEffect;
        public SoundDef breakSound = SoundDefOf.PsycastPsychicPulse;

        public float powerUsageBase = 300f;
        public float powerUsageFactorPassive = 0.1f;
        public float powerUsageFactorActive = 2.0f;
        public FloatRange powerUsageFactorScale = new FloatRange(1f, 10f);

        public float heatGenFactorPassive = 0.1f;
        public float heatGenFactorActive = 10.0f;

        public float stressPerDamage = 0.03f;
        public float shieldOverloadThreshold = 0.9f;
        public float shieldOverloadChance = 0.3f;

        public IntRange shieldScaleRange = new IntRange(3, 7);

        public Color shieldColour = Color.white;

        public bool podBlocker = true;
    }
}
