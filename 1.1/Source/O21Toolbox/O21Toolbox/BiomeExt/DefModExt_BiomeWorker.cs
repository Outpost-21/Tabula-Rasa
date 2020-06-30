using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class DefModExt_BiomeWorker : DefModExtension
    {
        public bool waterCovered = false;

        public bool useTemperature = false;
        public IntRange temperature = new IntRange(-9999, 9999);

        public bool useRainfall = false;
        public IntRange rainfall = new IntRange(0, 9999);

        public bool useSwampiness = false;
        public float swampiness = 0f;

        public BiomeUniversalCalculation calculation;
    }
}
