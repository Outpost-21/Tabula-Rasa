using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.PawnCrafter;

namespace O21Toolbox.PawnConverter
{
    public class ConverterProperties : DefModExtension
    {
        /// <summary>
        /// Sets if the converter requires power to process a pawn.
        /// </summary>
        public bool requiresPower = false; // Done.

        /// <summary>
        /// Not Implemented Yet. Setting for if the pawn can be forced into the building.
        /// </summary>
        public string forcedConversion = null; // Not Implemented.

        /// <summary>
        /// Not Implemented Yet. Setting for if the input pawn can be an animal.
        /// Requested feature.
        /// </summary>
        public string animalConversion = null; // Not Implemented.

        /// <summary>
        /// Sets the soundDef that plays when the converter is done.
        /// </summary>
        public SoundDef finishingSound = null; // Needs More Testing.

        /// <summary>
        /// Sets the Glower colour for when the converter is ready to be used.
        /// </summary>
        public ColorInt greenLight = new ColorInt(100, 255, 100, 0); // Needs More Testing.

        /// <summary>
        /// Sets the Glower colour for when the converter is NOT ready to be used.
        /// </summary>
        public ColorInt redLight = new ColorInt(252, 187, 113, 0); // Needs More Testing.
        
        /// <summary>
        /// If true the derived class will handle it.
        /// </summary>
        public bool customConversionTime = false;

        /// <summary>
        /// How many ticks are required to craft the pawn.
        /// </summary>
        public int ticksToConvert = 6000;

        /// <summary>
        /// Should the timer bar be visible? True or False.
        /// </summary>
        public bool timerBarEnabled = false; // Needs Testing.
        /// <summary>
        /// Size of the bar. (X, Y)
        /// </summary>
        public Vector2 timerBarSize = new Vector2(0.55f, 0.16f);
        /// <summary>
        /// Offset of the bar. (X, Y)
        /// </summary>
        public Vector2 timerBarOffset = new Vector2(0.0f, 0.0f);
        /// <summary>
        /// Colour for when the timer is filling in.
        /// </summary>
        public Color timerBarFill = new Color(0.9f, 0.9f, 0.10f);
        /// <summary>
        /// Colour for the empty bar.
        /// </summary>
        public Color timerBarUnfill = new Color(0.6f, 0.6f, 0.6f);
    }
}
