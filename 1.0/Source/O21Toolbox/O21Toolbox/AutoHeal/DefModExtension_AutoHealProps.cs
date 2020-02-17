using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutoHeal
{
    public class DefModExtension_AutoHealProps : DefModExtension
    {
        /// <summary>
        /// Time between heal ticks.
        /// </summary>
        public int healTicks = 1000;

        /// <summary>
        /// Whether or not parts can regrow.
        /// </summary>
        public bool regrowParts = true;

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
