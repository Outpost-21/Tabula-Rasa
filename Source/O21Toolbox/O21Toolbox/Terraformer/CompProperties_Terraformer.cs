using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class CompProperties_Terraformer : CompProperties
    {
        /// <summary>
        /// If null, no nodes will spawn from this. Otherwise a random node from the list is chosen.
        /// </summary>
        public List<ThingDef> nodeList = null;

        /// <summary>
        /// Maximum nodes which can spawn from this terraformer.
        /// 0 means no nodes can spawn.
        /// The Terraformer will attempt to spawn nodes in the most efficient layout.
        /// </summary>
        public int nodeMax = 0;

        /// <summary>
        /// Radius in which nodes can spawn.
        /// 0 means no nodes can spawn.
        /// </summary>
        public int nodeRange = 0;

        /// <summary>
        /// If true, this node will stop functioning without a connection to a sustainer.
        /// Sustainers can function through their child nodes, so can sustain indirectly.
        /// </summary>
        public bool failsWithoutParent = false;

        /// <summary>
        /// If true, a failing terraformer will restore the original terrain instead of spreading.
        /// </summary>
        public bool failureReverses = false;

        /// <summary>
        /// If true, this node will be able to sustain others linked to it.
        /// </summary>
        public bool canSustainNodes = true;

        /// <summary>
        /// The def listing used to determine what the terraformer changes, and what to.
        /// </summary>
        public TerraformerRulesDef terraformerRules = null;

        /// <summary>
        /// Radius which will be affected by the terraformer.
        /// </summary>
        public int terraformRange = 0;

        /// <summary>
        /// Whether or not the terraformer uses power.
        /// </summary>
        public bool requiresPower = false;
    }
}
