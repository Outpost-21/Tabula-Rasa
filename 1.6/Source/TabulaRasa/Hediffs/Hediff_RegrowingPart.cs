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
    public class Hediff_RegrowingPart : HediffWithComps
    {
        public DefModExt_RegrowingPart modExt;

        public DefModExt_RegrowingPart ModExt
        {
            get
            {
                if(modExt == null) { modExt = def.GetModExtension<DefModExt_RegrowingPart>(); }
                return modExt;
            }
        }

        public override bool ShouldRemove
        {
            get
            {
                return Severity >= def.maxSeverity;
            }
        }

        public override string Label => ModExt.labelText + Severity.ToStringPercent();

        public override void Tick()
        {
            base.Tick();
            Severity += (ModExt.growthPerDay / 60000) * (ModExt.coverageMultiplier ? Mathf.Clamp((1f - part.coverage), 0.1f, 1.0f) : 1f);
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
        }
    }
}
