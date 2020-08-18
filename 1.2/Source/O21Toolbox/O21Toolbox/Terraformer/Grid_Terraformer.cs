using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.Terraformer
{
    public class Grid_Terraformer : ICellBoolGiver, IExposable
    {
        public Map map;

        /// <summary>
        /// Cells which are potential candidates to grow into.
        /// </summary>
        public BoolGrid growToGrid;

        /// <summary>
        /// Cells which are considered already terraformed.
        /// </summary>
        public BoolGrid growFromGrid;

        /// <summary>
        /// Cells which are inside the growToGrid but adjacent to the growFromGrid;
        /// </summary>
        public BoolGrid growthEdgeGrid;

        public BoolGrid alwaysGrowFrom;

        public List<IntVec3> terraformerCells = new List<IntVec3>();

        public bool dirtyGrid = false;
        private readonly List<IntVec3> dirtyCells = new List<IntVec3>();

        public Color Color => Color.white;

        public Grid_Terraformer()
        {

        }

        public Grid_Terraformer(Map map)
        {
            this.map = map;
            growFromGrid = new BoolGrid(map);
            growToGrid = new BoolGrid(map);
            growthEdgeGrid = new BoolGrid(map);
            alwaysGrowFrom = new BoolGrid(map);
        }

        public void ExposeData()
        {

        }

        public bool GetCellBool(int index)
        {
            return growFromGrid[index] || growToGrid[index];
        }

        public Color GetCellExtraColor(int index)
        {
            throw new NotImplementedException();
        }

        public void MakeDirty(IntVec3 c)
        {
            foreach(var v in c.CellsAdjacent8Way(true))
            {
                if(v.InBounds(map) && !dirtyCells.Contains(v))
                {
                    dirtyCells.Add(v);
                }
            }
            dirtyGrid = true;
            UpdateDirty();
        }

        public void UpdateDirty()
        {
            if (!dirtyGrid)
            {
                return;
            }
            for (int i = dirtyCells.Count(); i > 0 ; i--)
            {
                IntVec3 cell = dirtyCells[i];
                SetEdgesBool(cell);
                SetGrowToBool(cell);
                SetGrowFromBool(cell);
                dirtyCells.Remove(cell);
            }
        }

        private void SetEdgesBool(IntVec3 c)
        {
            growthEdgeGrid[c] = !growFromGrid[c] && c.CellsAdjacent8Way().Any(v => v.InBounds(map) && growFromGrid[v]);
        }

        private void SetGrowFromBool(IntVec3 c)
        {
            growFromGrid[c] = c.CellsAdjacent8Way().All(v => v.InBounds(map) && !growToGrid[v]) || terraformerCells.Contains(c);
        }

        private void SetGrowToBool(IntVec3 c)
        {
            growToGrid[c] = map.GetComponent<MapComponent_Terraforming>().InRangeOfTerraformer(this, c);
        }
    }
}
