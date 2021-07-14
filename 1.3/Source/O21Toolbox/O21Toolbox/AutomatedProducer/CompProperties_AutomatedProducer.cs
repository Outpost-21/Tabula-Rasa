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
        public CompProperties_AutomatedProducer() => this.compClass = typeof(Comp_AutomatedProducer);

        /// <summary>
        /// Multiplies tick costs. Can be used to extend or shorten the time between production cycles, decreasing or increasing the speed of production.
        /// </summary>
        public float craftingTimeMultiplier = 1.0f;

        /// <summary>
        /// If limitOutput is true, this can be set to allow the producer to fill a stack before stopping.
        /// If something other than the current output is on the output tile it will still stop unless allowOverflow is true.
        /// </summary>
        public bool allowFullStack = true;

        /// <summary>
        /// If false, will not spawn more than a full stack in output.
        /// If true, will continue to spawn placing nearby.
        /// </summary>
        public bool allowOverflow = false;

        public List<RecipeDef_Automated> recipes = new List<RecipeDef_Automated>();
    }
}
