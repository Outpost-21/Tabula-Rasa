using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class DefModExt_BedExtensions : DefModExtension
    {
        /// <summary>
        /// Nullifies pawn being wet.
        /// </summary>
        public bool ignoreRain = false;

        /// <summary>
        /// Nullifies 'Slept Outside'.
        /// </summary>
        public bool ignoreOutdoors = false;

        /// <summary>
        /// Nullifies 'Slept In Cold'
        /// </summary>
        public bool ignoreCold = false;
        // Does it require power to ignore cold?
        public bool coldPowered = false;

        /// <summary>
        /// Nullifies 'Slept In Heat'
        /// </summary>
        public bool ignoreHeat = false;
        // Does it require power to ignore heat?
        public bool heatPowered = false;

        /// <summary>
        /// Nullifies 'Slept In Barracks'
        /// </summary>
        public bool ignoreOthers = false;
    }
}
