using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    //TODO: Remove next RW update
    public class DefModExt_PawnKindRaces : DefModExtension
    {
        /// <summary>
        /// Add a chance for the pawnKind to be a different race!
        /// Finally a balanced way to integrate into an existing faction with minimal effort!
        /// </summary>
        public List<WeightedRaceChoice> altRaces = new List<WeightedRaceChoice>();

    }
}
