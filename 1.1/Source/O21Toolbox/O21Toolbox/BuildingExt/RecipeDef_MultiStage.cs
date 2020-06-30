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
    public class RecipeDef_MultiStage : RecipeDef
    {
        /// <summary>
        /// List of graphics shown at different percentages, these are overlaid above the current stage.
        /// </summary>
        public List<MultiStageGraphics> stageGraphics = new List<MultiStageGraphics>();
    }

    public class MultiStageGraphics
    {
        /// <summary>
        /// At what percent to show this graphic.
        /// </summary>
        public float percentage;

        /// <summary>
        /// Which graphic to show.
        /// </summary>
        public GraphicData graphicData = new GraphicData();
    }
}
