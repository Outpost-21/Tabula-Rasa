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
    public class ShuttleDeathProperties
    {
        public ThingDef remainsDef;

        public bool crashOnDeath = false;

        public int crashLandRange = 0;

        public ShuttleExplosionProperties explosion;
    }
}
