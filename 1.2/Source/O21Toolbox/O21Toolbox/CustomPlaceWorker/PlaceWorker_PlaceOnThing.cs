using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomPlaceWorker
{
    public class PlaceWorker_PlaceOnThing : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool result = false;
            if(checkingDef.GetModExtension<DefModExtension_PlaceOnThing>() != null)
            {
                foreach (ThingDef targetThing in checkingDef.GetModExtension<DefModExtension_PlaceOnThing>().viableThings)
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

        /** public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == ThingDefOf.SteamGeyser;
        } **/
    }
}
