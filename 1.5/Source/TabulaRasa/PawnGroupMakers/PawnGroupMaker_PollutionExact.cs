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
    public class PawnGroupMaker_PollutionExact : PawnGroupMaker
    {
        public PollutionLevel pollutionLevel;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            if (Find.CurrentMap.TileInfo.PollutionLevel() == pollutionLevel)
            {
                return true;
            }
            return false;
        }
    }
}
