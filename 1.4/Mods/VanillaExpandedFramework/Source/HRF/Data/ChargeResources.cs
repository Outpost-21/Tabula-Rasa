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
    public class ChargeResources : IExposable
    {
        public ChargeResources()
        {

        }

        public List<ChargeResource> chargeResources = new List<ChargeResource>();
        public void ExposeData()
        {
            Scribe_Collections.Look(ref chargeResources, "chargeResources", LookMode.Deep);
        }
    }
}
