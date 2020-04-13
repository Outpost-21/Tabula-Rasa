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

        public int radius = 10;
        public bool radiusEditable = false;
        public IntRange radiusLimits = new IntRange(0, 100);

        public bool affectsAnimals = false;
        public float maxAnimalSizeAffected = 1.0f;
        public bool affectsPawns = false;
        public List<ThingDef> raceWhitelist = new List<ThingDef>();
        public List<ThingDef> raceBlacklist = new List<ThingDef>();
    }
}
