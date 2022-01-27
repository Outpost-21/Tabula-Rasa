using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Drones;
using O21Toolbox.SlotLoadable;

namespace O21Toolbox.PawnKindExt
{
    public class DefModExt_ExtendedPawnKind : DefModExtension
    {        
        /// <summary>
        /// Add a chance for the pawnKind to be a different race!
        /// Finally a balanced way to integrate into an existing faction with minimal effort!
        /// </summary>
        public List<ThingDefEntry> altRaces = new List<ThingDefEntry>();

        /// <summary>
        /// If true, will remove the chance for faction based race weighting to affect the pawnkind.
        /// </summary>
        public bool overrideFactionRaces = false;

        /// <summary>
        /// If true, makes all skills that aren't set specifically to zero.
        /// </summary>
        public bool flattenSkills = false;

        /// <summary>
        /// If True, removes all skill passions on generation.
        /// </summary>
        public bool clearPassions = false;

        /// <summary>
        /// If True, this removes all apparel at the end of generation, including that stupid fucking apparel Ideology forces onto pawns who SHOULD NOT HAVE IT!
        /// FUCK, TYNAN, WHY?
        /// </summary>
        public bool clearApparel = false;

        /// <summary>
        /// Sets skills to specific values.
        /// </summary>
        public List<SkillLevelSetting> skillSettings = new List<SkillLevelSetting>();

        /// <summary>
        /// For cases where vanillas options are worthless (nothing new there).
        /// </summary>
        public List<AdditionalHediffEntry> additionalHediffs = new List<AdditionalHediffEntry>();

        /// <summary>
        /// If true, the hediff will be randomly chosen from the list, if false, it will apply them all.
        /// </summary>
        public bool randomAdditionalHediff = false;

        /// <summary>
        /// If true, attempts to fill slottable weapons automatically,
        /// if true and used with slottableRestrictions correctly will
        /// also stick to the whitelist of what is allowed for this pawnKind.
        /// </summary>
        public bool slottableWeapon = false;

        /// <summary>
        /// Sets up restrictions on what is allowed to spawn in a specific slotLoadable.
        /// </summary>
        public List<SlottableRestrictions> slottableRestrictions = new List<SlottableRestrictions>();

        public bool clearChronicIllness = false;
        public bool clearAddictions = false;
        public bool replaceMissingParts = false;

    }

    public class SlottableRestrictions
    {
        public SlotLoadableDef slotLoadableDef;

        public List<ThingDef> slottableThingDefs = new List<ThingDef>();

        public bool canBeEmpty = false;

        public float chanceToFill = 1.0f;
    }

    public class AdditionalHediffEntry
    {
        public HediffDef hediff;

        public FloatRange severityRange = new FloatRange();

        public float weight;
    }

    public class ThingDefEntry
    {
        public List<ThingDef> races = new List<ThingDef>();

        public float weight;
    }
}
