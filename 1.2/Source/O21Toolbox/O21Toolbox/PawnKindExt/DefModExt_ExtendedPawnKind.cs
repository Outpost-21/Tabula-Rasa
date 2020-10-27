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
    }

    public class ThingDefEntry
    {
        public List<ThingDef> races = new List<ThingDef>();

        public float weight;
    }
}
