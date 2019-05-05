using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomPlaceWorker
{
    public class DefModExtension_PlaceNearThing : DefModExtension
    {
        /// <summary>
        /// Radius of effect.
        /// </summary>
        public int radius = 0;

        /// <summary>
        /// List of ThingDefs which the building must be placed near.
        /// </summary>
        public List<ThingDef> thingDefs = null;

        /// <summary>
        /// False means the building only needs to be placed near one of the listed items.
        /// True means the building must be placed within range of all of them.
        /// </summary>
        public bool allThings = false;

        /// <summary>
        /// This inverts the filter, meaning the building cannot be placed in the defined range of any listed items.
        /// </summary>
        public bool blacklist = false;

        /// <summary>
        /// If true, prevents the building from being placed at all.
        /// </summary>
        public bool preventPlacement = false;
    }
}
