using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class ApplyHediffs : IExposable
    {
        public float applyChance = 1.0f;
        public HediffDef hediffDef;
        public float severity = 1.0f;

        public void ExposeData()
        {
            Scribe_Values.Look(ref applyChance, "applyChance", -1.0f);
            Scribe_Values.Look(ref severity, "severity", 1.0f);
        }
    }
}
