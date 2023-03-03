using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
	public class PlaceWorker_ShowHediffAreaRadius : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			CompProperties_AdjustHediffsArea compProperties = def.GetCompProperties<CompProperties_AdjustHediffsArea>();
			if (compProperties != null)
			{
				var first = compProperties.resourceSettings.FirstOrDefault();
				if (first != null)
                {
                    GenDraw.DrawFieldEdges(GetAllCells(first, GenAdj.OccupiedRect(center, rot, def.size), def).ToList());
				}
			}
		}

        public HashSet<IntVec3> GetAllCells(ResourceProperties option, CellRect cellRect, ThingDef def)
        {
            if (option.worksThroughWalls)
            {
                return GetAllCellsInRadius(option, cellRect);
            }
            else
            {
                return GetAffectedCells(option, cellRect, def);
            }
        }
        public HashSet<IntVec3> GetAllCellsInRadius(ResourceProperties option, CellRect cellRect)
        {
            HashSet<IntVec3> tempCells = new HashSet<IntVec3>();
            foreach (var cell in cellRect.Cells)
            {
                foreach (var intVec in GenRadial.RadialCellsAround(cell, option.effectRadius, true))
                {
                    tempCells.Add(intVec);
                }
            }
            return tempCells;
        }
        public HashSet<IntVec3> GetAffectedCells(ResourceProperties option, CellRect cellRect, ThingDef def)
        {
            HashSet<IntVec3> affectedCells = new HashSet<IntVec3>();
            HashSet<IntVec3> tempCells = GetAllCellsInRadius(option, cellRect);

            Predicate<IntVec3> validator = delegate (IntVec3 cell)
            {
                if (!tempCells.Contains(cell)) return false;
                var edifice = cell.GetEdifice(Find.CurrentMap);
                var result = edifice == null || edifice.def.passability != Traversability.Impassable || edifice.def == def;
                return result;
            };
            var centerCell = cellRect.CenterCell;
            Find.CurrentMap.floodFiller.FloodFill(centerCell, validator, delegate (IntVec3 x)
            {
                if (tempCells.Contains(x))
                {
                    var edifice = x.GetEdifice(Find.CurrentMap);
                    var result = edifice == null || edifice.def.passability != Traversability.Impassable || edifice.def == def;
                    if (result && (GenSight.LineOfSight(centerCell, x, Find.CurrentMap) || centerCell.DistanceTo(x) <= 1.5f))
                    {
                        affectedCells.Add(x);
                    }
                }
            }, int.MaxValue, rememberParents: false, (IEnumerable<IntVec3>)null);
            affectedCells.AddRange(cellRect);
            return affectedCells;
        }
    }
}