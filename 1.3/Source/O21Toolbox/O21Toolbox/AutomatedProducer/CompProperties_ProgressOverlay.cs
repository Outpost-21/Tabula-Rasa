using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class CompProperties_ProgressOverlay : CompProperties
    {
        public CompProperties_ProgressOverlay()
        {
            compClass = typeof(Comp_ProgressOverlay);
        }

        public List<ProgressState> progressStates = new List<ProgressState>();

        public List<RecipeState> recipeStates = new List<RecipeState>();
    }

    public class ProgressState
    {
        public string texPath;

        public float progress;

        public bool flipForWest = true;
    }

    public class RecipeState
    {
        public string recipeDef;

        public List<ProgressState> states = new List<ProgressState>();
    }


}
