using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class CompProperties_AutomatedProducer : CompProperties
    {
        /// <summary>
        /// Multiplies tick costs. Can be used to extend or shorten the time between production cycles, decreasing or increasing the speed of production.
        /// </summary>
        public float craftingTimeMultiplier = 1.0f;

        /// <summary>
        /// If True, the producer will halt production if there is anything on the output tile.
        /// </summary>
        public bool limitOutput = false;

        /// <summary>
        /// If limitOutput is true, this can be set to allow the producer to fill a stack before stopping.
        /// If something other than the current output is on the output tile it will still stop.
        /// </summary>
        public bool allowFullStack = false;

        public List<RecipeDef_Automated> recipes = new List<RecipeDef_Automated>();
    }
}
