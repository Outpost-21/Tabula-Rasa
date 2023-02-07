using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld.Planet;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(FactionGiftUtility), "OfferGiftsCommand")]
    public static class Patch_FactionGiftUtility_OfferGiftsCommand
    {
        [HarmonyPostfix]
        public static void PostFix(Caravan caravan, Settlement settlement, ref Command __result)
        {
            if(settlement.Faction != null)
            {
                DefModExt_FactionExtension modExt = settlement.Faction.def.GetModExtension<DefModExt_FactionExtension>();
                if(modExt != null)
                {
                    if (!modExt.acceptsGifts)
                    {
                        if (modExt.acceptsGiftRange == null)
                        {
                            __result.Disable("TabulaRasa.FactionDoesNotAcceptGifts");
                        }
                        else if (settlement.Faction.GoodwillWith(Faction.OfPlayer) > modExt.acceptsGiftRange.Value.max || settlement.Faction.GoodwillWith(Faction.OfPlayer) < modExt.acceptsGiftRange.Value.min)
                        {
                            __result.Disable("TabulaRasa.FactionDoesNotAcceptGiftsGoodwill".Translate(modExt.acceptsGiftRange.Value.min, modExt.acceptsGiftRange.Value.max));
                        }
                    }
                }
            }
        }
    }
}
