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
            CompProperties_ApparelRestrict comp = equipment.GetCompProperties<CompProperties_ApparelRestrict>();
            if (comp.AllRequired && comp.RequiredApparel.All(x => IsWearing(x, pawn)))
            {
                return true;
            }
            if(comp.RequiredApparel.Any(x => IsWearing(x, pawn)))
            {
                return true;
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
