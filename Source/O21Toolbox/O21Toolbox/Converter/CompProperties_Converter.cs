using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Converter
{
    public class CompProperties_Converter : CompProperties
    {
        public CompProperties_Converter()
        {
            this.compClass = typeof(Comp_Converter);
        }

        public List<string> inputDefs = new List<string>();

        public PawnKindDef outputDef = null;

        public string requiredSex = "Any";

        public SoundDef finishingSound = null;

        public ColorInt redLight = new ColorInt(252, 187, 113, 0);

        public ColorInt greenLight = new ColorInt(100, 255, 100, 0);

        public int cookingTime = 1000;
    }
}
