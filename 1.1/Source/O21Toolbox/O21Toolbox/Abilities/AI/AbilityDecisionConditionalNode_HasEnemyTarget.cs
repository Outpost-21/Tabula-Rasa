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
    ///     Do our caster have a enemy target?
    /// </summary>
    public class AbilityDecisionConditionalNode_HasEnemyTarget : AbilityDecisionNode
    {
        public override bool CanContinueTraversing(Pawn caster)
        {
            var result = caster.mindState.enemyTarget != null;

            if (invert)
                return !result;

            return result;
        }
    }
}
