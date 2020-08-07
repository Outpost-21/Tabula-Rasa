using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class RestrictionCheck
    {
        public static bool CanWear(Apparel apparel, Pawn pawn)
        {
            ThingDef apparelDef = apparel.def;
            if(apparelDef == null || pawn == null)
            {
                return false;
            }
            if(apparelDef.HasModExtension<DefModExt_BodyRestrict>())
            {
                if (!apparelDef.GetModExtension<DefModExt_BodyRestrict>().BodyDefs.Contains(pawn.story.bodyType))
                {
                    return false;
                }
            }


            if (apparelDef.HasModExtension<DefModExt_ApparelRestrict>())
            {
                DefModExt_ApparelRestrict modExt = apparelDef.GetModExtension<DefModExt_ApparelRestrict>();
                if (modExt.allRequired && modExt.requiredApparel.All(x => IsWearing(x, pawn))) { }
                else if (modExt.requiredApparel.Any(x => IsWearing(x, pawn))) { }
                else if (modExt.requiredTag.Any(x => IsWearingTag(x, pawn))) { }
                else
                {
                    return false;
                }
            }

            Comp_Bondable bondableComp = apparel.TryGetComp<Comp_Bondable>();
            if (bondableComp != null && bondableComp.IsBonded)
            {
                if(bondableComp.BondedPawn == pawn)
                {
                    return false;
                }
            }

            return true;
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

        public static bool IsWearingTag(string tag, Pawn pawn)
        {
            for (int i = 0; i < pawn.apparel.WornApparelCount; i++)
            {
                if (pawn.apparel.WornApparel[i].def.apparel.tags.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
