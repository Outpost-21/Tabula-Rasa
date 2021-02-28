using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Requisition
{
    public class Comp_RequisitionTracker : ThingComp
    {
        public Pawn pawn;

        public List<RequisitionRank> requisitionRanks;
        public List<RequisitionRank> AllRanksForReading
        {
            get
            {
                return requisitionRanks;
            }
        }

        public Comp_RequisitionTracker()
        {
            this.pawn = this.parent as Pawn;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
