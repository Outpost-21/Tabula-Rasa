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
    public class ChargeResource : IExposable
    {
        public ChargeResource()
        {

        }

        public ChargeResource(float chargeResource, ChargeSettings chargeSettings)
        {
            this.chargeResource = chargeResource;
            this.chargeSettings = chargeSettings;
        }

        public float chargeResource;
        public ChargeSettings chargeSettings;
        public void ExposeData()
        {
            Scribe_Values.Look(ref chargeResource, "chargeResource");
            Scribe_Deep.Look(ref chargeSettings, "chargeSettings");
        }
    }
}
