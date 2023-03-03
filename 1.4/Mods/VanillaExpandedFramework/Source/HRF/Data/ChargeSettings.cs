using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    public class ChargeSettings : IExposable
    {
        public HediffResourceDef hediffResource;
        public float resourcePerCharge = -1f;
        public float damagePerCharge = -1f;
        public float minimumResourcePerUse = -1f;
        public DamageScalingMode? damageScaling;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref hediffResource, "hediffResource");
            Scribe_Values.Look(ref resourcePerCharge, "resourcePerCharge");
            Scribe_Values.Look(ref damagePerCharge, "damagePerCharge");
            Scribe_Values.Look(ref minimumResourcePerUse, "minimumResourcePerUse");
            Scribe_Values.Look(ref damageScaling, "damageScaling");
        }
    }
}
