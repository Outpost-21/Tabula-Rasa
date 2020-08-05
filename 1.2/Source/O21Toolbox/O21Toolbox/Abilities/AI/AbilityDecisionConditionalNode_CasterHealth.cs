using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities.AI
{
    /// <summary>
    ///     Compares the casters health.
    /// </summary>
    public class AbilityDecisionConditionalNode_CasterHealth : AbilityDecisionNode
    {
        public float maxHealth = 1.0f;
        public float minHealth = 0.0f;

        public override bool CanContinueTraversing(Pawn caster)
        {
            var result = caster.HealthScale >= minHealth &&
                         caster.health.summaryHealth.SummaryHealthPercent <= maxHealth;

            if (invert)
                return !result;

            return result;
        }
    }
}
