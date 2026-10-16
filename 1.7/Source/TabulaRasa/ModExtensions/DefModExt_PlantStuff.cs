
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
        /// Defines min distance to nearest other plant of the same def when naturally generated.
        /// </summary>
        public float distToNearestOther = 11.3f;

        /// <summary>
        /// Unlike the vanilla "diesUnderLight" setting, this specifically finds sunlight (including from sunlamps) not normal light.
        /// </summary>
        public bool diesInSunlight = false;

        /// <summary>
        /// If a plant typically is set to die in light, this being set to false will prevent it from being affected by Darklight.
        /// If set to true darklight exclusively will kill the plant.
        /// </summary>
        public bool? diesInDarklight = false;
    }
}
