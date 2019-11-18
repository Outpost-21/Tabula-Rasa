using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponRestrict
{
    public class DefModExt_ApparelRestrict : DefModExtension
    {
        public List<ThingDef> requiredApparel = null;

        public bool allRequired = false;
    }
}
