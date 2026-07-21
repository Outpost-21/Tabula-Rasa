using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{

    [Flags]
    public enum ShuttleState
    {
        All = 0,
        Landed = 1,
        TakingOff = 2,
        Landing = 3,
        Flight = 4,
        Hover = 5,
        Travel = 6
    }
}
