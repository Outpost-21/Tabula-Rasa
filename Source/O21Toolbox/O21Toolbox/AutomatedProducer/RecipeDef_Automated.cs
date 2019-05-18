using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class RecipeDef_Automated : Def
    {
        /// <summary>
        /// Set amount of time between production cycles.
        /// </summary>
        public int baseTickCost = 0;

        /// <summary>
        /// If true, will use the randomCostMin and randomCostMax to determin a random timer for the next cycle.
        /// Using this obviously ignores the set baseTickCost.
        /// </summary>
        public bool randomTickCost = false;

        /// <summary>
        /// Minimum random tick cost time.
        /// </summary>
        public int randomCostMin = 1000;

        /// <summary>
        /// Maximum random tick cost time.
        /// </summary>
        public int randomCostMax = 10000;

        /// <summary>
        /// ThingDefs which need to be placed inside the producer to begin crafting process.
        /// If null, the producer will not craft but instead function as a passive producer.
        /// </summary>
        public List<ThingDefCountClass> costs = null;

        /// <summary>
        /// Recipe Output. Can be multiple products, but if the producer output is limited to only filling the designated tile then it will ignore any aside from the first.
        /// </summary>
        public List<ThingDefCountClass> products = null;

        /// <summary>
        /// Randomly selects which product to output. All products are weighted equally.
        /// </summary>
        public bool randomizeProducts = false;

        /// <summary>
        /// If set, will require research before the recipe is visible.
        /// </summary>
        public ResearchProjectDef requiredResearch = null;

        public RecipeInfo recipeInfo = new RecipeInfo();
    }

    public class RecipeInfo
    {
        public string recipeType = null;

        public string recipeIcon = null;

        public string productionString = "Producing: ";
    }
}
