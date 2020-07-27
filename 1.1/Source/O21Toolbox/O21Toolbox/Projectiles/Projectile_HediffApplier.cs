using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.Projectiles
{
    public class Projectile_HediffApplier : Projectile
    {
        public DefModExt_HediffApplier modExt => this.def.GetModExtension<DefModExt_HediffApplier>();

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if (modExt != null)
            {
                HediffApplier.ApplyHediff(hitThing, modExt);
            }
        }
    }
}
