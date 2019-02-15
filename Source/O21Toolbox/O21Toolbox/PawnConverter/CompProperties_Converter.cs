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

        /// <summary>
        /// Sets if the converter requires power to process a pawn.
        /// </summary>
        public bool requiresPower = false; // Done.

        /// <summary>
        /// Viable inputs for conversion. Uses the defName of the race itself. Should be able to accept any pawn race if left 'null'.
        /// </summary>
        public List<string> inputDefs = null; // Done.

        /// <summary>
        /// Target output for converts. Uses pawnKinds to retrieve a target race as well as generating the random options for other variables.
        /// Doesn't change race at all if left 'null'.
        /// </summary>
        public PawnKindDef outputDef = null; // Done.

        /// <summary>
        /// Sets if the input pawn needs to be a specific sex. Accepts Male or Female, leaving 'null' will ignore the requirement.
        /// </summary>
        public string requiredSex = null; // Done.

        /// <summary>
        /// Sets required hediffs, all listed hediffs are required not just one or some.
        /// </summary>
        public List<HediffDef> requiredHediffs = null; // Needs More Testing.

        /// <summary>
        /// Removes the hediffs listed in requiredHediffs during conversion.
        /// </summary>
        public bool removeRequiredHediffs = false; // Needs More Testing.

        /// <summary>
        /// Hediffs to force onto the output pawn, if they already have the hediff it will not be applied.
        /// </summary>
        public Hediff forcedHediff = null; // Needs More Testing.

        /// <summary>
        /// Changes sex/gender of the pawn during conversion. Accepts Male or Female.
        /// </summary>
        public string outputSex = null; // Needs More Testing.

        /// <summary>
        /// Forces the body type to whatever is defined, requires an actual bodyType like Thin, Fat, Male, Female or Hulk.
        /// </summary>
        public BodyTypeDef forcedBody = null; // Needs More Testing.

        /// <summary>
        /// Forces the crownType of a pawn, must match the texture exactly. Writing "RANDOM" instead will use the new random pawns head type, useful for if there are no direct matches.
        /// </summary>
        public string forcedHead = null; // Needs More Testing.

        /// <summary>
        /// Sets if to change skin colour during conversion. 
        /// Use this OR randomSkinColor, not both.
        /// </summary>
        public bool forcedSkinColor = false; // Needs More Testing.

        /// <summary>
        /// Sets the skin colour to a random selection from the appropriate race skin colours.
        /// Use this OR forcedSkinColor, not both.
        /// </summary>
        public bool randomSkinColor = false; // Needs More Testing.

        /// <summary>
        /// Defines colours to use if forcedSkinColor is set to True.
        /// </summary>
        public Color forcedSkinColorOne = new Color(0, 0, 0); // Needs More Testing.
        public Color forcedSkinColorTwo = new Color(0, 0, 0); // Needs More Testing.

        /// <summary>
        /// Use either randomHair or forcedHair, not both.
        /// randomHair is a True or False option on if the converter should choose at random from race appropriate hairs.
        /// </summary>
        public bool randomHair = false;
        /// <summary>
        /// Use either randomHair or forcedHair, not both.
        /// forcedHair is a defined hairDef to choose, must match exactly.
        /// </summary>
        public HairDef forcedHair = null; // Needs More Testing.

        /// <summary>
        /// Sets if to randomize hair colour. Do not use with forcedHairColor.
        /// </summary>
        public bool randomHairColor = false; // Needs More Testing.

        /// <summary>
        /// Sets if to force the hair colour. Do not use with randomHairColor.
        /// </summary>
        public bool forcedHairColor = false; //Needs More Testing.
        public bool forcedHairColorOne = false; // Needs More Testing.
        public bool forcedHairColorTwo = false; // Needs More Testing.

        /// <summary>
        /// Sets the colours for if forcedHairColor is True.
        /// </summary>
        public Color hairColorOne = new Color(0, 0, 0); // Needs More Testing.
        public Color hairColorTwo = new Color(0, 0, 0); // Needs More Testing.

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
        /// Sets amount of time pawns will be in the chamber for during conversion. 
        /// Keep in mind if they leave the chamber during this time it will cancel the conversion.
        /// One Rimworld day is 60000 (60k).
        /// </summary>
        public int cookingTime = 1000; // Needs More Testing.
        
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
