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
    public class HediffSeverityPairing
    {
        public HediffDef hediff;

        /// <summary>
        /// Initial severity when the hediffs are applied.
        /// </summary>
        public float severityInitial = 0.01f;

        /// <summary>
        /// Severity increase if the pawn already has the hediff.
        /// </summary>
        public float severityIncrease = 0.01f;
    }
}
