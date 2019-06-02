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
    public class Trigger_HostileToColony : Trigger
    {
        public const int checkInterval = GenTicks.TicksPerRealSecond + 2;

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if ((signal.type == TriggerSignalType.Tick)
                && (Find.TickManager.TicksGame % checkInterval == 0))
            {
                if (lord.faction.HostileTo(Faction.OfPlayer))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
