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
    public class DefModExt_ThingProducer : DefModExtension
    {
        public ThingDef thingDef = null;

        public int productionTime = 1000;

        public int maxThings = 1;

        public string retrievalString = "Take Item";

        public bool requiresPower = false;
    }
}
