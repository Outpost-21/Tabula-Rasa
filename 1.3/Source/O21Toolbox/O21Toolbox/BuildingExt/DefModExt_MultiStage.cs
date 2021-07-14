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
    public class DefModExt_MultiStage : DefModExtension
    {
        /// <summary>
        /// If true, this will check for if the space where the next stage would end up is empty.
        /// Usually this will be false, if you want to replace the current stage with the next.
        /// </summary>
        public bool requiresEmptySpace = false;

        /// <summary>
        /// If true, this stage is destroyed when the next stage finishes.
        /// Usually this will be true, if you want to replace the current stage with the next.
        /// </summary>
        public bool destroyWhenFinished = true;

        /// <summary>
        /// List of available recipes for the next stage.
        /// </summary>
        public List<RecipeDef_MultiStage> recipes = new List<RecipeDef_MultiStage>();
    }
}
