using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class DefModExt_ApparelRestrict : DefModExtension
    {
        public List<ThingDef> requiredApparel = new List<ThingDef>();

        public List<string> requiredTag = new List<string>();

        public bool allRequired = false;
    }
}
