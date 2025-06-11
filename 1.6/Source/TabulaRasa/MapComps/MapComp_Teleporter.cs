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
    public class MapComp_Teleporter : MapComponent
    {
        public List<Thing> allMapTeleports = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref allMapTeleports, "allMapTeleports", LookMode.Reference);
        }

        public MapComp_Teleporter(Map map) : base(map)
        {

        }

        public void RegisterTeleporter(Thing thing)
        {
            if (!allMapTeleports.Contains(thing))
            {
                allMapTeleports.Add(thing);
            }
        }

        public void UnregisterTeleporter(Thing thing)
        {
            if (allMapTeleports.Contains(thing))
            {
                allMapTeleports.Remove(thing);
            }
        }
    }
}
