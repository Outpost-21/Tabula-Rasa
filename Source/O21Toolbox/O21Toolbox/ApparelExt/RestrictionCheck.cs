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
        public static bool CanWear(ThingDef apparel, Pawn pawn)
        {
            if(apparel.HasModExtension<DefModExt_BodyRestrict>())
            {
                if (!apparel.GetModExtension<DefModExt_BodyRestrict>().BodyDefs.Contains(pawn.story.bodyType))
                {
                    return false;
                }
            }


            if (apparel.HasModExtension<DefModExt_ApparelRestrict>())
            {
                DefModExt_ApparelRestrict modExt = apparel.GetModExtension<DefModExt_ApparelRestrict>();
                if (modExt.allRequired && modExt.requiredApparel.All(x => IsWearing(x, pawn)))
                {
                }
                else if (modExt.requiredApparel.Any(x => IsWearing(x, pawn)))
                {
                }
                else
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
    }
}
