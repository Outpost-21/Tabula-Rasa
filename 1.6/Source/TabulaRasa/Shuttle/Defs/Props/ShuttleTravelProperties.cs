using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleTravelProperties
    {
        public ThingDef skyfallerDef;

        public WorldObjectDef worldObjectDef;

        public float speed = 0.00025f;

        public float fuelConsumption = 10f;

        public string travelLabel = "TabulaRasa.ShuttleTravelLabel";

        public string travelDesc = "TabulaRasa.ShuttleTravelDesc";

        public string travelIcon = "TabulaRasa/UI/Shuttles/Travel";
    }
}
