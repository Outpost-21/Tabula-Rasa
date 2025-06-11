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
    public class MiningSettings : IExposable
    {
        public Comp_Mining parent;

        public MiningFilter filter;

        public MiningSettings()
        {
            this.filter = new MiningFilter();
        }

        public MiningSettings(Comp_Mining q)
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
