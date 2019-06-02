using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public static class Util_JobDefOf
    {
        public static JobDef UseOrbitalRelayConsole => DefDatabase<JobDef>.GetNamed("O21Toolbox_JobDef_UseOrbitalRelayConsole");
        public static JobDef TradeWithCargoSpaceship => DefDatabase<JobDef>.GetNamed("O21Toolbox_JobDef_TradeWithCargoSpaceship");
        public static JobDef RequestSpaceshipTakeOff => DefDatabase<JobDef>.GetNamed("O21Toolbox_JobDef_RequestSpaceshipTakeOff");
        public static JobDef BoardSpaceship => DefDatabase<JobDef>.GetNamed("O21Toolbox_JobDef_BoardSpaceship");
        public static JobDef CarryDownedPawn => DefDatabase<JobDef>.GetNamed("O21Toolbox_JobDef_CarryDownedPawn");
    }
}
