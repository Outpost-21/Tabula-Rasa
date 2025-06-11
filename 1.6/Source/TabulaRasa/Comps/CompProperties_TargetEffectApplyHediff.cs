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
    public class CompProperties_TargetEffectApplyHediff : CompProperties
    {
        public CompProperties_TargetEffectApplyHediff()
        {
            compClass = typeof(CompTargetEffect_ApplyHediff);
        }

        public HediffDef hediff;
    }
}
