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
        /// Time between heal ticks, lower number means faster healing.
        /// </summary>
        public int healTicks = 1000;

        /// <summary>
        /// Time between growth ticks, lower number means faster growth.
        /// </summary>
        public int growthTicks = 200;

        /// <summary>
        /// Text displayed to describe the growth.
        /// </summary>
        public string growthText = "Growth: ";

        public HediffDef protoBodyPart = null;

        public HediffDef curedBodyPart = null;
    }
}
