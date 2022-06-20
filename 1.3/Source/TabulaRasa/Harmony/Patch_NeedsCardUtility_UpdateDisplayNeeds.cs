using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(NeedsCardUtility), "UpdateDisplayNeeds")]
    public static class Patch_NeedsCardUtility_UpdateDisplayNeeds
    {
        public static string foodNeedLabel;
        public static string foodNeedDescription;

        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            if (foodNeedLabel.NullOrEmpty())
            {
                foodNeedLabel = NeedDefOf.Food.cachedLabelCap;
            }

            if (foodNeedDescription.NullOrEmpty())
            {
                foodNeedDescription = NeedDefOf.Food.description;
            }

            if (pawn != null && pawn.def.HasModExtension<DefModExt_FoodNeedAdjuster>())
            {
                DefModExt_FoodNeedAdjuster modExt = pawn.def.GetModExtension<DefModExt_FoodNeedAdjuster>();
                if (modExt.newLabel != null)
                {
                    NeedDefOf.Food.cachedLabelCap = modExt.newLabel;
                }

                if (modExt.newDescription != null)
                {
                    NeedDefOf.Food.description = modExt.newDescription;
                }
            }
            else
            {
                NeedDefOf.Food.cachedLabelCap = foodNeedLabel;
                NeedDefOf.Food.description = foodNeedDescription;
            }
        }
    }
}
