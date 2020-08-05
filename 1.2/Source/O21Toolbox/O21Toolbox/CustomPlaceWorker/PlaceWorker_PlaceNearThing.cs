using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomPlaceWorker
{
    public class PlaceWorker_PlaceNearThing : PlaceWorker
    {
        private static List<IntVec3> checkableCells = new List<IntVec3>();

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            bool result = false;
            if (checkingDef.GetModExtension<DefModExtension_PlaceNearThing>() != null)
            {
                if (!checkingDef.GetModExtension<DefModExtension_PlaceNearThing>().preventPlacement)
                {
                    return true;
                }
                foreach (ThingDef targetThing in checkingDef.GetModExtension<DefModExtension_PlaceNearThing>().thingDefs)
                {
                    foreach (things)
                        Thing thing = map.thingGrid.ThingAt(loc, targetThing);

                        if (thing == null || thing.Position != loc)
                        {
                        }
                        else if (thing != null || thing.Position == loc)
                        {
                            result = true;
                        }
                }

                if (result != true)
                {
                    return "Must be placed near specific thing(s), check building details for more info.";
                }
                return true;
            }
            return "Building has PlaceWorker_ProximityRestriction but lacks a DefModExtension_ProximityRestriction to tell it what it can place on.";
        }

        public List<IntVec3> CheckableCells(BuildableDef checkingDef, IntVec3 pos, Map map)
        {
            checkableCells.Clear();
            if (!pos.InBounds(map))
            {
                return checkableCells;
            }
            Region region = pos.GetRegion(map, RegionType.Set_Passable);
            if (region == null)
            {
                return checkableCells;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, 
                delegate (Region r)
                {
                    foreach (IntVec3 item in r.Cells)
                    {
                        if (item.InHorDistOf(pos, checkingDef.GetModExtension<DefModExtension_PlaceNearThing>().radius))
                        {
                            checkableCells.Add(item);
                        }
                    }
                    return false;
                }, 
                13, RegionType.Set_Passable);
        }

        public bool CheckAreaForThing(int radius, ThingDef thing)
        {
            return false;
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            Map currentMap = Find.CurrentMap;
            GenDraw.DrawFieldEdges(WatchBuildingUtility.CalculateWatchCells(def, center, rot, currentMap).ToList<IntVec3>());
        }
    }
}
