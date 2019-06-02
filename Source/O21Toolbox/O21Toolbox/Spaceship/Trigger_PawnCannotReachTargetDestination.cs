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
    public class Trigger_PawnCannotReachTargetDestination : Trigger
    {
        public const int checkInterval = GenTicks.TicksPerRealSecond + 3;

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if ((signal.type == TriggerSignalType.Tick)
                && (Find.TickManager.TicksGame % checkInterval == 0))
            {
                IntVec3 targetDestination = (lord.LordJob as LordJob_SpaceshipBase).targetDestination;
                foreach (Pawn pawn in lord.ownedPawns)
                {
                    if (pawn.CanReach(targetDestination, PathEndMode.OnCell, Danger.Some) == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
