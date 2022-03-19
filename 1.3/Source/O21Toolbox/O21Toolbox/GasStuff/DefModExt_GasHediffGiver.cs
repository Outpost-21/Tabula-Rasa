using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.GasStuff
{
    public class DefModExt_GasHediffGiver : DefModExtension
    {
        public float radius;
        public HediffDef hediffDef;
        public int ticksBeforeApply;
        public int adjustSeverity;
        public bool checkToxicSensitivity = true;
    }
}
