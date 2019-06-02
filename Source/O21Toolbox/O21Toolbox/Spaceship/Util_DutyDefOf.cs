using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Spaceship
{
    public static class Util_DutyDefOf
    {
        public static DutyDef DutyBoardSpaceship => DefDatabase<DutyDef>.GetNamed("O21Toolbox_DutyDef_BoardSpaceship");
        public static DutyDef CarryDownedPawn => DefDatabase<DutyDef>.GetNamed("O21Toolbox_DutyDef_CarryDownedPawn");
        public static DutyDef EscortCarrier => DefDatabase<DutyDef>.GetNamed("O21Toolbox_DutyDef_EscortCarrier");
        public static DutyDef HealColonists => DefDatabase<DutyDef>.GetNamed("O21Toolbox_DutyDef_HealColonists");
    }
}
