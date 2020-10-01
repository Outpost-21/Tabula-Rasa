using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class CompProperties_TraitsOverTime : CompProperties
    {
        public CompProperties_TraitsOverTime()
        {
            this.compClass = typeof(Comp_TraitsOverTime);
        }

        /// <summary>
        /// Maximum traits that can be added at any given time.
        /// </summary>
        public int maxTraits = 3;

        /// <summary>
        /// Randomly set time period between traits being added.
        /// 60,000 is an in-game day.
        /// </summary>
        public IntRange timeBetweenTraits = new IntRange(30000, 60000);

        /// <summary>
        /// List of traits which can randomly be assigned.
        /// Mutually exclusive with traitBlacklist, whitelist takes priority if both exist.
        /// </summary>
        public List<Utility.TraitEntry> traitWhitelist = new List<Utility.TraitEntry>();

        /// <summary>
        /// List of traits which cannot be randomly assigned.
        /// </summary>
        public List<Utility.TraitEntry> traitBlacklist = new List<Utility.TraitEntry>();
    }
}
