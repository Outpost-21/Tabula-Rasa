using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class CompProperties_OrganicBuilding : CompProperties
    {
        public CompProperties_OrganicBuilding() => this.compClass = typeof(Comp_OrganicBuilding);

        /// <summary>
        /// Whether or not the building will passively heal any damage.
        /// </summary>
        public bool canHeal = false;

        /// <summary>
        /// Number of ticks between each heal attempt.
        /// </summary>
        public int ticksBetweenHeal = 100;

        /// <summary>
        /// Number of ticks between each wither attempt.
        /// </summary>
        public int ticksBetweenWither = 100;

        /// <summary>
        /// How much power is drawn to heal (-1 means it doesn't need power to heal).
        /// </summary>
        public int healingPowercost = -1;

        /// <summary>
        /// Whether or not a maintainer is needed to heal.
        /// </summary>
        public bool needsMaintainerToHeal = false;

        /// <summary>
        /// Whether or not the building will wither away without connection to a maintainer.
        /// </summary>
        public bool needsMaintainer = false;

        /// <summary>
        /// Whether or not the building needs power to maintain itself.
        /// </summary>
        public bool needsPower = false;

        /// <summary>
        /// Whether or not the building can maintain others.
        /// </summary>
        public bool isMaintainer = false;

        /// <summary>
        /// Whether or not the building draws power to maintain others.
        /// </summary>
        public bool needsPowerToMaintain = false;

        /// <summary>
        /// Radius in which this building will maintain others.
        /// </summary>
        public float maintainerRadius = 0f;

    }
}
