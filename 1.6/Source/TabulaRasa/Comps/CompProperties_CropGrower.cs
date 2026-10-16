using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Comps
{
    public class CompProperties_CropGrower : CompProperties
    {
        public CompProperties_CropGrower()
        {
            compClass = typeof(Comp_CropGrower);
        }

        public bool exportWhenComplete;

        public Vector2 plantGrid;

        public Vector2 plantOffset;

        public List<string> allowedTags;

        /// <summary>
        /// Always drawn, useful for glass rendering above plants and similar.
        /// </summary>
        public GraphicData overlayGraphicData;

        /// <summary>
        /// Overlay drawn when machine is inactive.
        /// </summary>
        public GraphicData inactiveGraphicData;

        /// <summary>
        /// Overlay drawn when the machine is running.
        /// </summary>
        public GraphicData activeGraphicData;

        /// <summary>
        /// Overlay drawn when the machine is done and awaiting extraction.
        /// Only applies when machine is set up to not automatically export.
        /// </summary>
        public GraphicData completeGraphicData;
    }
}
