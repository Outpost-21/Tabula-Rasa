using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    public abstract class Verb_AreaEffect : BaseVerb
    {
        public override bool TryCastShot()
        {
            foreach (var cell in GenRadial.RadialCellsAround(currentTarget.Cell, Props.effectRadius, true)
                .Where(cell => cell.InBounds(caster.Map)))
                AffectCell(cell);
            return true;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            return Props.effectRadius;
        }

        protected abstract void AffectCell(IntVec3 cell);
    }
}