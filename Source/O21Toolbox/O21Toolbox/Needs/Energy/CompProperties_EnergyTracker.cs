using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Properties for the EnergyTracker
    /// </summary>
    public class CompProperties_EnergyTracker : CompProperties
    {
        public CompProperties_EnergyTracker()
        {
            compClass = typeof(Comp_EnergyTracker);
        }

        /// <summary>
        /// Can the thing hibernate at specific points?
        /// </summary>
        public bool canHibernate = true;

        /// <summary>
        /// Job to give when hibernating.
        /// </summary>
        public JobDef hibernationJob;

        /// <summary>
        /// Def for the energy need used.
        /// </summary>
        public NeedDef energyNeedDef = null;

        /// <summary>
        /// Can the thing self destruct?
        /// </summary>
        public bool canSelfDestruct = false;

        /// <summary>
        /// Label for Self Destruct Gizmo
        /// </summary>
        public string selfDestructLabel = "O21SelfDestructLabel";

        /// <summary>
        /// Description for Self Destruct Gizmo
        /// </summary>
        public string selfDestructDesc = "O21SelfDestructDesc";

        /// <summary>
        /// Icon texture path for self destruct gizmo
        /// </summary>
        public string selfDestructIcon = "UI/Commands/Detonate";
    }
}
