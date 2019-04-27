using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomPlaceWorker
{
    public class DefModExtension_PlaceOnThing : DefModExtension
    {
        /// <summary>
        /// List of ThingDefs which the building can be place on.
        /// </summary>
        public List<ThingDef> viableThings;
    }
}
