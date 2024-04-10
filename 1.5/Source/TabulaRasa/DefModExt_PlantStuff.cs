
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class DefModExt_PlantStuff : DefModExtension
    {
        /// <summary>
        /// Defines if a plant can spawn or be planted on normal water.
        /// </summary>
        public bool freshWaterPlant = false;

        /// <summary>
        /// Defines if a plant can spawn or be planted on ocean water.
        /// </summary>
        public bool oceanWaterPlant = false;

        /// <summary>
        /// Defines min distance to nearest other plant of the same def.
        /// </summary>
        public float distToNearestOther = 11.3f;
    }
}
