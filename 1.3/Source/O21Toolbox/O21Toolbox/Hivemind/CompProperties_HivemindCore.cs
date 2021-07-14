using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Hivemind
{
    public class CompProperties_HivemindCore : CompProperties
    {
        public CompProperties_HivemindCore() => this.compClass = typeof(Comp_HivemindCore);

        /// <summary>
        /// Tag used to decide which hivemind this pawn can connect to.
        /// </summary>
        public string hivemindTag;

        /// <summary>
        /// Hediff used to determine states the pawn is in while connected and disconnected.
        /// </summary>
        public HediffDef hivemindCoreHediff;

        /// <summary>
        /// Curve with severity and an integer. This will increase/decrease severity if disconnected/connected.
        /// </summary>
        public SimpleCurve severityAtCount;

        /// <summary>
        /// The type of hivemind determines if they can remain connected while on a different map from the connected core.
        /// </summary>
        public HivemindConnection hivemindConnection = HivemindConnection.local;
    }
}
