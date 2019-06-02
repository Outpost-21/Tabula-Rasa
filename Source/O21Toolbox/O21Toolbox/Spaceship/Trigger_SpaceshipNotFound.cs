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
    public class Trigger_SpaceshipNotFound : Trigger
    {
        public const int checkInterval = GenTicks.TicksPerRealSecond + 4;

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if ((signal.type == TriggerSignalType.Tick)
                && (Find.TickManager.TicksGame % checkInterval == 0))
            {
                foreach (Thing thing in (lord.LordJob as LordJob_SpaceshipBase).targetDestination.GetThingList(lord.Map))
                {
                    if (thing is Spaceship_Building)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
