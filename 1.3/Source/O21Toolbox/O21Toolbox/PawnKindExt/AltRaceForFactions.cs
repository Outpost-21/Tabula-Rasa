using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnKindExt
{
    public class AltRaceForFactions : Def
    {
        public List<AltRaceOption> altRaceOptions;
    }

    public class AltRaceOption
    {
        public ThingDef raceDef;
        public FactionDef faction;
        public float weight;
    }
}
