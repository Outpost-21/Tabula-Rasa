using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace O21Toolbox.Spaceship
{
    public class JobGiver_RunToCarrier : JobGiver_Wander
    {
        public JobGiver_RunToCarrier()
        {
            this.wanderRadius = 5f;
            this.locomotionUrgency = LocomotionUrgency.Sprint;
        }
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            const int numberOfCellsAhead = 15;
            IntVec3 targetCell = pawn.mindState.duty.focus.Cell;
            Pawn carrier = (pawn.GetLord().CurLordToil as LordToil_EscortDownedPawn).Data.carrier;
            if ((carrier.pather != null)
                && carrier.pather.Moving
                && (carrier.pather.curPath != null)
                && (carrier.pather.curPath.NodesLeftCount > numberOfCellsAhead))
            {
                targetCell = carrier.pather.curPath.Peek(numberOfCellsAhead);
            }
            return targetCell;
        }
    }
}
