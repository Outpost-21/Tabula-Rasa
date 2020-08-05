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
    ///     Checks whether the caster is equipped with a ranged weapon or not.
    /// </summary>
    public class AbilityDecisionConditionalNode_UsingRangedWeapon : AbilityDecisionNode
    {
        public override bool CanContinueTraversing(Pawn caster)
        {
            var result = caster?.equipment.Primary != null && caster.equipment.Primary.def.IsRangedWeapon;

            if (invert)
                return !result;

            return result;
        }
    }
}
