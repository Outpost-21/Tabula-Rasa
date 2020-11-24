using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Drones;

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
        /// If true, makes all skills that aren't set specifically to zero.
        /// </summary>
        public bool flattenSkills = false;

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
