using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelRestrict
{
    public class RestrictionCheck
    {
        public static bool CanWear(ThingDef apparel, BodyTypeDef bodyType)
        {
            if(apparel.GetCompProperties<CompProperties_BodyRestrict>() != null)
            {
                if (!apparel.GetCompProperties<CompProperties_BodyRestrict>().BodyDefs.Contains(bodyType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
