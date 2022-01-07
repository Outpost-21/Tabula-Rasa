using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class Comp_Homing : ThingComp
    {
        public int lastRecalculationTickCount;
        public bool isHoming;
        public IntVec3 lastCell;

        public CompProperties_Homing Props => (CompProperties_Homing)this.props;

        public bool IsHoming
        {
            get => this.isHoming;
            set => this.isHoming = value;
        }

        public IntVec3 LastCell
        {
            get => this.lastCell;
            set => this.lastCell = value;
        }

        public bool ShouldRecalculateNow()
        {
            lastRecalculationTickCount++;

            if(lastRecalculationTickCount >= Props.recalculationInterval)
            {
                return true;
            }

            return false;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref this.isHoming, "isHoming", false);
            Scribe_Values.Look(ref lastRecalculationTickCount, "lastRecalculationTickCount", 0);
        }
    }
}
