using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace O21Toolbox
{
    [HarmonyPatch(typeof(FoodUtility), "WillIngestStackCountOf")]
    public class Patch_FoodUtility_WillIngestStackCountOf
    {
        [HarmonyPrefix]
        public static bool Prefix(int __result, ref Pawn ingester, ref ThingDef def)
        {
            if (ingester == null)
                return true;

            bool haveNeed = ingester?.needs.TryGetNeed(NeedDefOf.Food) != null;

            if (!haveNeed)
            {
                __result = 0;
                return false;
            }

            return true;
        }
    }
}
