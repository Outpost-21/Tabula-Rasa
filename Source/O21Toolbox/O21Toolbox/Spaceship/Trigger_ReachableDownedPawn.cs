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
    public class Trigger_ReachableDownedPawn : Trigger
    {
        public const int checkInterval = GenTicks.TicksPerRealSecond + 1;

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if ((signal.type == TriggerSignalType.Tick)
                && (Find.TickManager.TicksGame % checkInterval == 0))
            {
                IntVec3 targetDestination = (lord.LordJob as LordJob_SpaceshipBase).targetDestination;
                // Look for a reachable unreserved downed pawn.
                if (lord.ownedPawns.NullOrEmpty())
                {
                    lord.Cleanup();
                    return false;
                }
                Pawn pawnToRescue = Util_Spaceship.GetRandomReachableDownedPawn(lord.ownedPawns.RandomElement());
                if (pawnToRescue == null)
                {
                    return false;
                }
                // Check all lord pawns can reach downed pawn.
                foreach (Pawn pawn in lord.ownedPawns)
                {
                    if (pawn.CanReserveAndReach(pawnToRescue, PathEndMode.OnCell, Danger.Some) == false)
                    {
                        return false;
                    }
                }
                // Check all lord pawns can reach target destination.
                bool targetDestinationIsReachable = true;
                foreach (Pawn pawn in lord.ownedPawns)
                {
                    if (pawn.CanReach(targetDestination, PathEndMode.OnCell, Danger.Some) == false)
                    {
                        targetDestinationIsReachable = false;
                        break;
                    }
                }
                if (targetDestinationIsReachable)
                {
                    return true;
                }
                // Try to find a new exit spot.
                IntVec3 exitSpot = IntVec3.Invalid;
                bool exitSpotIsValid = Util_Spaceship.TryFindRandomExitSpot(lord.Map, lord.ownedPawns.RandomElement().Position, out exitSpot);
                if (exitSpotIsValid)
                {
                    targetDestinationIsReachable = true;
                    foreach (Pawn pawn in lord.ownedPawns)
                    {
                        if (pawn.CanReach(exitSpot, PathEndMode.OnCell, Danger.Some) == false)
                        {
                            targetDestinationIsReachable = false;
                            break;
                        }
                    }
                }
                return targetDestinationIsReachable;
            }
            return false;
        }
    }
}
