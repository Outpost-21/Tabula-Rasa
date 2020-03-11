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
    public class HeatNet
    {
        public HeatNetManager heatNetManager;

        public List<Comp_ShieldBuilding> shieldComps = new List<Comp_ShieldBuilding>();

        public List<Comp_HeatRelease> ventComps = new List<Comp_HeatRelease>();

        private bool IsPowerUser(Comp_ShieldBuilding sb)
        {
            return sb.HasPowerTrader;
        }

        private void DistributeHeatAmongReleasers(float heat)
        {
            if(heat <= 0f || !ventComps.Any<Comp_HeatRelease>())
            {

            }
        }
    }
}
