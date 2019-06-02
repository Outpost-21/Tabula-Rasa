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
    public class TransitionAction_CheckExitSpotIsValid : TransitionAction
    {
        public override void DoAction(Transition trans)
        {
            Lord lord = trans.target.lord;
            IntVec3 targetDestination = (lord.LordJob as LordJob_SpaceshipBase).targetDestination;
            bool needNewExitSpot = false;
            bool isEdgeCell = targetDestination.InBounds(lord.Map)
                && ((targetDestination.x == 0)
                    || (targetDestination.x == lord.Map.Size.x - 1)
                    || (targetDestination.z == 0)
                    || (targetDestination.z == lord.Map.Size.z - 1));
            if (isEdgeCell == false)
            {
                needNewExitSpot = true;
            }
            else
            {
                foreach (Pawn pawn in lord.ownedPawns)
                {
                    if (pawn.CanReach(targetDestination, PathEndMode.OnCell, Danger.Some) == false)
                    {
                        needNewExitSpot = true;
                        break;
                    }
                }
            }

            IntVec3 newTargetDestination = targetDestination;
            if (needNewExitSpot)
            {
                if (Util_Spaceship.TryFindRandomExitSpot(lord.Map, lord.ownedPawns.RandomElement().Position, out newTargetDestination) == false)
                {
                    newTargetDestination = CellFinder.RandomEdgeCell(lord.Map);
                }
            }
            (lord.LordJob as LordJob_SpaceshipBase).targetDestination = newTargetDestination;
            // Refresh lord toil destination anyway as it may have been initialized with an invalid vector (case of a fallback).
            LordToil_Travel travelToil = trans.target as LordToil_Travel;
            if (travelToil != null)
            {
                travelToil.SetDestination(newTargetDestination);
            }
            LordToil_EscortDownedPawn escortToil = trans.target as LordToil_EscortDownedPawn;
            if (escortToil != null)
            {
                escortToil.SetDestination(newTargetDestination);
            }
        }
    }
}
