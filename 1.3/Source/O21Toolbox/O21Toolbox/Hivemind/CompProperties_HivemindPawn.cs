using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Hivemind
{
    public class CompProperties_HivemindPawn : CompProperties
    {
        public CompProperties_HivemindPawn() => this.compClass = typeof(Comp_HivemindPawn);

        /// <summary>
        /// Tag used to decide which hivemind this pawn can connect to.
        /// </summary>
        public string hivemindTag;

        /// <summary>
        /// Hediff used to determine states the pawn is in while connected and disconnected.
        /// </summary>
        public HediffDef connectedHediff;
        public HediffDef disconnectedHediff;

        /// <summary>
        /// The type of core to connect to, this limits it if there is only going to be one option to connect to so there isn't an unnecessary option.
        /// </summary>
        public HivemindType hivemindType = HivemindType.either;

        public string autoConnectTexPath = null;
        public string connectTexPath = null;
        public string disconnectTexPath = null;
    }
}
