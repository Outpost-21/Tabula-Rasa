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
    public class DefModExt_EnergyNeed : DefModExtension
    {
        public bool canConsumeBatteries = true;
        public bool canChargeWirelessly = true;
        public bool canChargeFromSocket = true;
        public bool canHibernate = true;
    }
}
