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
    public class RequisitionRank : IExposable
    {
        public RequisitionRankDef def;

        public Faction faction;

        public Pawn pawn;

        public int tickEarned;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref tickEarned, "tickEarned");
        }
    }
}
