using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomDispenser
{
    public class DefModExt_CustomDispenser : DefModExtension
    {
        public List<ThingDef> thingDefs = new List<ThingDef>();

        public bool requiresPower = false;

        public float powerPerUse = 0;

        public SoundDef dispenseSound = SoundDefOf.HissSmall;
    }
}
