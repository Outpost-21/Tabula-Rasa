using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class DefModExt_HediffApplier : DefModExtension
    {
        public HediffDef hediff;

        public float chanceToApply = 1.0f;

        public float severityIncreasePerShot = 0.0f;

        public float initialSeverity = 0.5f;
    }
}
