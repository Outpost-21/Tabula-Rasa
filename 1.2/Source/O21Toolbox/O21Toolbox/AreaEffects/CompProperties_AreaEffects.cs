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
        /// If true the thing will look for pawns in the room to apply to, ignoring the radius. If false it will use the radius.
        /// If true and no room is detected, it will default to radius, but if the radius is not defined it will do nothing.
        /// </summary>
        public bool roomBased = true;

        /// <summary>
        /// If true, and roomBased is true, room detection will check for a roof.
        /// </summary>
        public bool roomRequiresRoof = true;

        /// <summary>
        /// Radius to apply effect to.
        /// </summary>
        public int radius = 0;

        /// <summary>
        /// Hediffs to apply while pawns are within the same room or radius.
        /// </summary>
        public List<HediffSeverityPairing> applyHediffs = new List<HediffSeverityPairing>();

        /// <summary>
        /// Adjustable time between each running of the code.
        /// </summary>
        public int ticksBetweenRuns = 250;
    }

    public class HediffSeverityPairing
    {
        public HediffDef hediff;

        /// <summary>
        /// Initial severity when the hediffs are applied.
        /// </summary>
        public float severityInitial = 0.01f;

        /// <summary>
        /// Severity increase if the pawn already has the hediff.
        /// </summary>
        public float severityIncrease = 0.01f;
    }
}
