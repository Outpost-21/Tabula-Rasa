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
    ///     Is the pawn in combat or near hostiles?
    /// </summary>
    public class AbilityDecisionConditionalNode_InCombat : AbilityDecisionNode
    {
        public override bool CanContinueTraversing(Pawn caster)
        {
            var result = caster.mindState.anyCloseHostilesRecently;

            if (invert)
                return !result;

            return result;
        }
    }
}
