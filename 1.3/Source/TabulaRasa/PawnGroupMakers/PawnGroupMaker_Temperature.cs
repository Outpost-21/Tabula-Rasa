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
    public class PawnGroupMaker_Temperature : PawnGroupMaker
    {
        public float minTemperature = -999f;
        public float maxTemperature = 999f;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            if (Find.CurrentMap.mapTemperature.OutdoorTemp >= minTemperature && Find.CurrentMap.mapTemperature.OutdoorTemp <= maxTemperature)
            {
                return true;
            }
            return false;
        }
    }
}
