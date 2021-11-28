using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.Health
{
    public class Comp_UseHealthPack : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            AutoHeal.HealUtility.TrySealWounds(usedBy, new List<HediffDef>());
        }
    }
}
