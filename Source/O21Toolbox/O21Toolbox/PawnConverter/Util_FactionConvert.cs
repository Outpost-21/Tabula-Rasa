using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using O21Toolbox.Utility;

namespace O21Toolbox.PawnConverter
{
    public class Util_FactionConvert
    {
        public static Pawn FactionConvert(Pawn pawn, bool makeFriendly, float chanceToBerserk, string reason)
        {
            if (makeFriendly)
            {
                if (pawn.guest != null)
                {
                    pawn.guest.SetGuestStatus(null, false);
                }
                pawn.SetFaction(pawn.Faction);
                pawn.workSettings.EnableAndInitialize();
            }
            if (chanceToBerserk > 0.0f)
            {
                if (Rand.Chance(chanceToBerserk))
                {
                    MentalBreakDefOf.Berserk.Worker.TryStart(pawn, reason, false);
                }
            }
            return pawn;
        }
    }
}
