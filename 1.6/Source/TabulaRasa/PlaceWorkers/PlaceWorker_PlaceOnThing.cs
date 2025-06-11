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
    public class PlaceWorker_PlaceOnThing : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool result = false;
            if (checkingDef.GetModExtension<DefModExt_PlaceOnThing>() != null)
            {
                foreach (ThingDef targetThing in checkingDef.GetModExtension<DefModExt_PlaceOnThing>().viableThings)
                {
                    Thing thingie = map.thingGrid.ThingAt(loc, targetThing);

                    if (thingie == null || thingie.Position != loc)
                    {
                    }
                    else if (thingie != null || thingie.Position == loc)
                    {
                        result = true;
                    }
                }

                if (result != true)
                {
                    return "Must be placed on specific thing(s), check building details for more info.";
                }
                return true;
            }
            return "Building has PlaceWorker_PlaceOnThing but lacks a DefModExtension_PlaceOnThing to tell it what it can place on.";
        }
    }
}
