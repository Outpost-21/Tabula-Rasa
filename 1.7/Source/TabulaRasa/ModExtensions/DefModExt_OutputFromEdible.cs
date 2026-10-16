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
    public class DefModExt_OutputFromEdible : DefModExtension
    {
        public ThingDef outputThing;

        public float chance = 1f;

        public int multiplier = 1;

        public GeneDef geneRequired;
    }
}
