using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    public class DefModExt_SolarNeed : DefModExtension
    {
        /// <summary>
        /// Rate at which the food meter fills in sunlight.
        /// </summary>
        public float absorbRate = 1.0f;

        /// <summary>
        /// Whether or not artificial lights will work for the process.
        /// </summary>
        public bool naturalOnly = false;

        /// <summary>
        /// Rate at which the meter fills in artificial sunlight.
        /// </summary>
        public float artificialRate = 0.5f;

        /// <summary>
        /// Minimum artificial light needed.
        /// </summary>
        public float minArtificialGlow = 0.3f;

        /// <summary>
        /// Optimal artificial light needed.
        /// </summary>
        public float optimalArtificialGlow = 1.0f;

        /// <summary>
        /// Rate at which the food meter drains.
        /// </summary>
        public float consumeRate = 1.0f;

        /// <summary>
        /// Whether or not the food meter will drain at night.
        /// </summary>
        public bool restAtNight = false;
    }
}
