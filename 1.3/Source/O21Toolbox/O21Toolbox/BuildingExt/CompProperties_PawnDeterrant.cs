using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class CompProperties_PawnDeterrant : CompProperties
    {
        public CompProperties_PawnDeterrant() => this.compClass = typeof(Comp_PawnDeterrant);

        public int ticksBetweenRuns = 600;
        public int radius = 10;
        public int minFleeDistance = 20;
        public bool affectsAnimals = false;
        public bool affectsPawns = false;
        public bool affectsColony = false;
        public float maxBodySizeAffected = 1.0f;
        public List<ThingDef> raceWhitelist = new List<ThingDef>();
        public List<ThingDef> raceBlacklist = new List<ThingDef>();
    }
}
