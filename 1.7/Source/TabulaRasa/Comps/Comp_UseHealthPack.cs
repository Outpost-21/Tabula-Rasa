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
    public class Comp_UseHealthPack : CompUseEffect
    {
        public new CompProperties_UseHealthPack Props => (CompProperties_UseHealthPack)props;

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            HealthUtil.TrySealWounds(usedBy, Props.ignoredHediffs);
        }
    }
}
