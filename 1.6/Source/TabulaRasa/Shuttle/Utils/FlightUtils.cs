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
    public static class FlightUtils
    {
        public static Thing GetTarget(List<Thing> ListThing)
        {
            Thing result = null;
            for (int i = 0; i < ListThing.Count; i++)
            {
                if (ListThing[i] is Pawn)
                {
                    result = ListThing[i];
                    break;
                }
                if (ListThing[i] is Building && ListThing[i].def.useHitPoints)
                {
                    result = ListThing[i];
                    break;
                }
            }
            return result;
        }
    }
}
