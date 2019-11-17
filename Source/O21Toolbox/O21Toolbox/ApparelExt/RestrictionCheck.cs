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
        public static bool CanWear(ThingDef apparel, BodyTypeDef bodyType)
        {
            if(apparel.HasModExtension<DefModExt_BodyRestrict>())
            {
                if (!apparel.GetModExtension<DefModExt_BodyRestrict>().BodyDefs.Contains(bodyType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
