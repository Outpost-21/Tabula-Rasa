using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Conveyors
{
    public class CompProperties_ConveyorBelt : CompProperties
    {
        /// <summary>
        /// Time between movement ticks. Higher number means slower belts.
        /// </summary>
        public int movementDelay = 60;

        /// <summary>
        /// Whether or not belts which are backstuffed stack matching items together.
        /// </summary>
        public bool backStacking = false;

        /// <summary>
        /// Stack percentage limit. Prevents any more than the given percentage of an item be moved per movement tick.
        /// If set to 0 or 1, will allow a full stack to move.
        /// </summary>
        public float stackLimit = 0f;

        /// <summary>
        /// Whether or not the conveyor needs power.
        /// </summary>
        public bool requiresPower = true;

        /// <summary>
        /// Tiles accepted for input. Multiple viable inputs will choose one at random unless given a preference.
        /// </summary>
        public List<Vector3> inputTiles = null;

        /// <summary>
        /// Tiles accepted for output. Multiple viable outputs will round-robin unless given a preverence.
        /// </summary>
        public List<Vector3> outputTiles = null;
    }
}