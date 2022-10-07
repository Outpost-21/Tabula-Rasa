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
    public class PlaceWorker_PlaceNearThing : PlaceWorker
    {
        List<Thing> buildingsInRange = new List<Thing>();

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            DefModExt_PlaceNearThing modExt = checkingDef.GetModExtension<DefModExt_PlaceNearThing>();
            if (modExt != null)
            {
                foreach (ThingDef targetThing in checkingDef.GetModExtension<DefModExt_PlaceNearThing>().thingDefs)
                {
                    foreach (Thing building in map.listerThings.ThingsOfDef(targetThing))
                    {
                        if (loc.DistanceTo(building.Position) < modExt.radius)
                        {
                            buildingsInRange.Add(building);
                        }
                    }
                    foreach (Thing building in map.listerThings.ThingsOfDef(targetThing.blueprintDef))
                    {
                        if (loc.DistanceTo(building.Position) < modExt.radius)
                        {
                            buildingsInRange.Add(building);
                        }
                    }
                }

                if (!modExt.blacklist && buildingsInRange.NullOrEmpty())
                {
                    return "Must be placed near specific thing(s), check building description for more info.";
                }

                if (modExt.blacklist && !buildingsInRange.NullOrEmpty())
                {
                    return "Must be placed away from specific thing(s), check building description for more info.";
                }
                return true;
            }
            return "Building has PlaceWorker_PlaceNearThing but lacks a DefModExt_PlaceNearThing to tell it what it can or cannot place nearby.";
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);
            DefModExt_PlaceNearThing modExt = def.GetModExtension<DefModExt_PlaceNearThing>();
            if (modExt != null && modExt.radius > 0)
            {
                GenDraw.DrawRadiusRing(center, modExt.radius);
            }
        }
    }
}
