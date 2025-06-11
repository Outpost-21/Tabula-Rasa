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
    public class CompProperties_AutoResearch : CompProperties
    {
        public CompProperties_AutoResearch()
        {
            this.compClass = typeof(Comp_AutoResearch);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);

            if (pawnKind == null)
            {
                pawnKind = PawnKindDefOf.Colonist;
            }
            if(xenotype == null)
            {
                xenotype = XenotypeDefOf.Baseliner;
            }
        }

        public bool requiresPower = false;

        public bool totalPawnsAffectSpeed = false;

        public float bonusPerPawn = 0.1f;

        public float researchSpeedFactor = 1.0f;

        public PawnKindDef pawnKind;

        public XenotypeDef xenotype;
    }
}
