using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Basically tags a ThingDef as a mechanical pawn.
    /// </summary>
    public class ArtificialPawnProperties : DefModExtension
    {
        /// <summary>
        /// If true the pawn will not lose any skill due to decay.
        /// </summary>
        public bool noSkillLoss = true;

        /// <summary>
        /// Can this Droid be social?
        /// </summary>
        public bool canSocialize = false;

        /// <summary>
        /// Does the colony care if they die?
        /// </summary>
        public bool colonyCaresIfDead = false;

        /// <summary>
        /// Def for applicable repair parts (medicine)
        /// </summary>
        public List<ThingDef> repairParts = null;
    }
}
