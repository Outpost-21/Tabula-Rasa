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
    ///     Checks the amount of cover the enemy target is in.
    /// </summary>
    public class AbilityDecisionConditionalNode_EnemyTargetCover : AbilityDecisionNode
    {
        /// <summary>
        ///     Maximum amount of cover to return true at.
        /// </summary>
        public float maxCover = 1.0f;

        /// <summary>
        ///     Minimum amount of cover to return true at.
        /// </summary>
        public float minCover = 0.0f;

        public override bool CanContinueTraversing(Pawn caster)
        {
            if (caster.mindState.enemyTarget == null)
                return false;

            var cover = CoverUtility.CalculateOverallBlockChance(caster.mindState.enemyTarget.Position, caster.Position,
                caster.Map);

            var result = cover >= minCover && cover < maxCover;

            if (invert)
                return !result;

            return result;
        }
    }
}
