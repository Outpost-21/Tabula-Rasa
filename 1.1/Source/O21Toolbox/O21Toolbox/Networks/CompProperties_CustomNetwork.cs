using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class CompProperties_CustomNetwork : CompProperties
    {
        public NetworkDef networkDef;

        public bool canDistribute;

        public float baseNetworkConsumption;

        public bool shortCircuitInRain;

        public SoundDef soundNetworkOn;

        public SoundDef soundNetworkOff;

        public SoundDef soundAmbientOn;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            foreach(StatDrawEntry s in base.SpecialDisplayStats(req))
            {
                yield return s;
            }
            if(this.baseNetworkConsumption > 0f)
            {
                yield return new StatDrawEntry(StatCategoryDefOf.Building, "Network Consumption: ".ToString(), this.baseNetworkConsumption.ToString("F0") + this.networkDef.networkMeasurement, "", 0);
            }
            yield break;
        }
    }
}
