using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Drones
{
    public static class Drone_Utility
    {
        public static bool IsPlayerControlledDrone(this Thing thing)
        {
            return thing is Drone_Pawn p && p.Faction.IsPlayer;
        }
    }
}
