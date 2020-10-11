﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class DefModExt_StoneTypeOverride : DefModExtension
    {
        public List<ThingDef> thingDefs = new List<ThingDef>();

        public bool removeOriginals = false;
    }
}
