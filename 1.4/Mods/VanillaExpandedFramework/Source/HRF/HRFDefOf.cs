using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    [DefOf]
    public static class HRFDefOf
    {
        static HRFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HRFDefOf));
        }

        public static StatDef HFR_StuffDamageFactor;
        public static StatDef HFR_CaravanSpeedFactor;
        public static StatDef HFR_CaravanDifficultyFactor;
    }
}
