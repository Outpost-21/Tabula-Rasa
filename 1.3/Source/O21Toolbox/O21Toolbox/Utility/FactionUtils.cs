using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

using O21Toolbox.FactionExt;

namespace O21Toolbox.Utility
{
    [StaticConstructorOnStartup]
    public static class FactionUtils
    {
        static FactionUtils()
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
