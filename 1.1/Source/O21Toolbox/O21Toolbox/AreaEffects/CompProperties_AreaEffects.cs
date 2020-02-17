using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AreaEffects
{
    public class CompProperties_AreaEffects : CompProperties
    {
        /// <summary>
        /// If true the thing will look for pawns in the room to apply to, if false it will use the radius.
        /// If true and no room is detected, it will default to radius, but if the radius is not defined it will do nothing.
        /// </summary>
        public bool roomBased = true;

        /// <summary>
        /// Radius to apply effect to.
        /// </summary>
        public int radius = 0;

        /// <summary>
        /// Hediffs to apply while pawns are withing the same room or radius.
        /// </summary>
        public List<HediffDef> applyHediffs = new List<HediffDef>();
    }
}
