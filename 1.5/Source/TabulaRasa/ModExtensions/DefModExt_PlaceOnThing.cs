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
    public class DefModExt_PlaceOnThing : DefModExtension
    {

        /// <summary>
        /// List of ThingDefs which the building can be place on.
        /// </summary>
        public List<ThingDef> viableThings;
    }
}
