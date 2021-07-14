using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Automation
{
    public class MiningSettings : IExposable
    {
        public Comp_Quarry parent;

        public MiningFilter filter;

        public MiningSettings()
        {
            this.filter = new MiningFilter();
        }

        public MiningSettings(Comp_Quarry q)
        {
            parent = q;
            filter = new MiningFilter();
        }

        public void CopyFrom(MiningSettings other)
        {
            filter.CopyAllowancesFrom(other.filter);
        }

        public void ExposeData()
        {
            Scribe_Deep.Look<MiningFilter>(ref this.filter, "filter");
        }
    }
}
