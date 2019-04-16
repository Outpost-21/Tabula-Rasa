using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    class EnergyHediffs : DefModExtension
    {
        /// <summary>
        /// Hediff for low power.
        /// </summary>
        public HediffDef coolantLoss = null;

        /// <summary>
        /// Hediff for low power.
        /// </summary>
        public HediffDef powerShortage = null;

        /// <summary>
        /// Hediff for no power.
        /// </summary>
        public HediffDef powerFailure = null;
    }
}
