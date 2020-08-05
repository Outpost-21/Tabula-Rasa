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
    public class Biome_Temperature : DefModExtension
    {
        public float tempOffset = 0f;
        public float tempStableWeight = 0f;
        public float tempStableValue = 0f;
    }
}
