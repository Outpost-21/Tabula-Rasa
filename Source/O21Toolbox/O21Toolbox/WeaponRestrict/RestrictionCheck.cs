using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponRestrict
{
    public class RestrictionCheck
    {
        public static bool CanEquip(ThingDef equipment, Pawn pawn)
        {
            if (equipment.HasModExtension<DefModExt_ApparelRestrict>())
            {
                DefModExt_ApparelRestrict modExt = equipment.GetModExtension<DefModExt_ApparelRestrict>();
                if (modExt.allRequired && modExt.requiredApparel.All(x => IsWearing(x, pawn)))
                {
                    return true;
                }
                if (modExt.requiredApparel.Any(x => IsWearing(x, pawn)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWearing(ThingDef apparel, Pawn pawn)
        {
            for (int i = 0; i < pawn.apparel.WornApparelCount; i++)
            {
                if (pawn.apparel.WornApparel[i].def == apparel)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
