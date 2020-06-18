using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class SpawnThings : IExposable
    {
        public ThingDef def;
        public FactionDef factionDef;
        public PawnKindDef kindDef;
        public int spawnCount = 1;
        public bool temporary;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Defs.Look(ref kindDef, "kindDef");
            Scribe_Defs.Look(ref factionDef, "factionDef");
            Scribe_Values.Look(ref spawnCount, "spawnCount", 1);
            Scribe_Values.Look(ref temporary, "temporary", false);
        }
    }
}
