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
    public class DefModExt_AutoHealProps : DefModExtension
    {
        /// <summary>
        /// Time between heal ticks.
        /// </summary>
        public int healTicks = 1000;

        /// <summary>
        /// List of Hediffs ignored when healing.
        /// </summary>
        public List<HediffDef> ignoreWhenHealing = new List<HediffDef>();

        /// <summary>
        /// Whether or not parts can regrow.
        /// </summary>
        public bool regrowParts = true;

        /// <summary>
        /// Time between cure ticks.
        /// </summary>
        public int cureTicks = 1000;

        /// <summary>
        /// Whether or not diseases are cured.
        /// </summary>
        public bool removeInfections = true;

        /// <summary>
        /// List of infections which are not cured if removeInfections is true.
        /// </summary>
        public List<HediffDef> infectionsAllowed = new List<HediffDef>();

        /// <summary>
        /// Sometimes the automatic detection doesn't work as intended, such as mechanites in vanilla.
        /// You can use this list to tell the code to remove specific hediffs as part of the "curing" process.
        /// </summary>
        public List<HediffDef> explicitRemovals = new List<HediffDef>();

        /// <summary>
        /// Time between growth ticks.
        /// </summary>
        public int growthTicks = 1000;

        /// <summary>
        /// Text displayed to describe the growth.
        /// </summary>
        public string growthText = "Growth: ";

        public HediffDef protoBodyPart = null;

        public HediffDef curedBodyPart = null;

        public HediffDef autoHealHediff = null;
    }
}
