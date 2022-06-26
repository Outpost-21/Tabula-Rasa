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
    public class DefModExt_PawnKindExtended : DefModExtension
    {
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

        public bool clearChronicIllness = false;
        public bool clearAddictions = false;
        public bool replaceMissingParts = false;
    }
}
