using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Laser
{
    public static class ThingExtensions
    {
        public static bool IsShielded(this Thing thing)
        {
            return (thing as Pawn).IsShielded();
        }
        
        public static bool IsShielded(this Pawn pawn)
        {
            if (pawn == null || pawn.apparel == null) return false;

            DamageInfo damageTest = new DamageInfo(DamageDefOf.Bomb, 0f, 0f, -1, null);
            foreach(Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel.CheckPreAbsorbDamage(damageTest)) return true;
            }

            return false;
        }
    }
}
