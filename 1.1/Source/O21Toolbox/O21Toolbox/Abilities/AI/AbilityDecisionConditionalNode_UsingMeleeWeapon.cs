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
    ///     Checks whether the caster is equipped with a melee weapon or not.
    /// </summary>
    public class AbilityDecisionConditionalNode_UsingMeleeWeapon : AbilityDecisionNode
    {
        public bool countUnarmed = true;

        public override bool CanContinueTraversing(Pawn caster)
        {
            var result = false;

            if (countUnarmed)
                result = caster?.equipment.Primary == null ||
                         caster?.equipment.Primary != null && !caster.equipment.Primary.def.IsRangedWeapon;
            else
                result = caster?.equipment.Primary != null && !caster.equipment.Primary.def.IsRangedWeapon;

            if (invert)
                return !result;

            return result;
        }
    }
}
