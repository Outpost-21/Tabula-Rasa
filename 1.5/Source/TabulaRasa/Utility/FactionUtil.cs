using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    [StaticConstructorOnStartup]
    public static class FactionUtil
    {
        static FactionUtil()
        {

        }

        public static int FactionPoints()
        {
            return (int)Find.World.worldObjects.Settlements.Sum(delegate (Settlement s)
            {
                DefModExt_FactionExtension modExt = s.def.GetModExtension<DefModExt_FactionExtension>();
                int? num = (modExt != null) ? new int?(modExt.spreadsMoreSettlements) : null;
                return (num != null) ? ((float)num.GetValueOrDefault()) : 0f;
            });
        }
    }
}
