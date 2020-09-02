using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.CustomDropPod
{
    public class DefModExt_CustomDropPods : DefModExtension
    {
        public IncidentDef customIncident;

        public List<CustomPodGroups> groups = new List<CustomPodGroups>();
    }

    public class CustomPodGroups
    {
        /// <summary>
        /// Def for the pod that drops. Skyfaller is automatically obtained from adding "_Skyfaller" to the end.
        /// </summary>
        public ThingDef customPod;

        /// <summary>
        /// Optional defined cost for a podGroup, it will otherwise use the combatPower of pawns it can choose from.
        /// </summary>
        public int raidCost = -1;

        /// <summary>
        /// List of pawnkinds for the group, and how many it attempts to fill the pod with.
        /// </summary>
        public List<PawnKindCount> pawnKinds = new List<PawnKindCount>();
    }
}
