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
    public class CompProperties_VerbSwitch : CompProperties
    {
        public CompProperties_VerbSwitch()
        {
            this.compClass = typeof(Comp_VerbSwitch);
        }

        /// <summary>
        /// Research required to be able to switch modes.
        /// </summary>
        public ResearchProjectDef requiredResearch;

        /// <summary>
        /// Research required for a specific verb.
        /// </summary>
        public List<VerbSwitchPair> requiredResearchSpecific = new List<VerbSwitchPair>();

        /// <summary>
        /// If True, will initiate cooldown after switch.
        /// </summary>
        public bool useCooldown = true;
    }
}
