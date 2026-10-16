using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class DefModExt_Switcher : DefModExtension
    {
        public ThingDef buildingDef;

        public SoundDef activateSound;

        public string label = "Switch";

        public string description = "Switch the building to the target def.";

        public string icon;
    }
}
