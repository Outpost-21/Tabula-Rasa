using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Animation
{
    public class AnimationWorker_DustPuffs : AnimationWorker
    {
        public override void MoteItUp(Thing thing, int frame = 0)
        {
            base.MoteItUp(thing, frame);

            CellRect cellRect = GenAdj.OccupiedRect(thing.Position, thing.Rotation, thing.def.size);
            cellRect = cellRect.ExpandedBy(1);

            foreach (IntVec3 cell in cellRect.Cells)
            {
                FleckMaker.ThrowDustPuff(cell, thing.Map, 2f);
            }
        }
    }
}
