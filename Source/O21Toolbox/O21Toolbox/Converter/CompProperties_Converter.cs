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

        public bool requiresPower = false; // Done.

        public List<string> inputDefs = null; // Done.

        public PawnKindDef outputDef = null; // Change to only swap race rather than replace pawn.

        public string requiredSex = null; // Done.

        public List<HediffDef> requiredHediffs = null; // Needs Testing.

        public Hediff forcedHediff = null; // Needs Testing.

        public string outputSex = null; // Not Implemented.

        public string forcedBody = null; // Needs Testing.

        public string forcedHead = null; // Not Implemented.

        public string forcedSkinColor = null; // Not Implemented.

        public ColorInt forcedSkinColorInt = new ColorInt(0, 0, 0, 0); // Not Implemented.

        public string forcedHair = null; // Not Implemented.

        public string forcedHairColor = null; // Not Implemented.

        public ColorInt forcedHairColorInt = new ColorInt(0, 0, 0, 0); // Not Implemented.

        public string forcedConversion = null; // Not Implemented.

        public string animalConversion = null; // Not Implemented.

        public SoundDef finishingSound = null; // Done.

        public ColorInt greenLight = new ColorInt(100, 255, 100, 0); // Done.

        public ColorInt redLight = new ColorInt(252, 187, 113, 0); // Done.

        public int cookingTime = 1000; // Done.
    }
}
