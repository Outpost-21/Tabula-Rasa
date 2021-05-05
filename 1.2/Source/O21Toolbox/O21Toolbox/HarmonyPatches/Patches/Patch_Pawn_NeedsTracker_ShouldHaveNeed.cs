using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;

namespace O21Toolbox.HarmonyPatches.Patches
{

	[HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
	public class Patch_Pawn_NeedsTracker_ShouldHaveNeed
	{
		[HarmonyPostfix]
		public static void Postfix(NeedDef nd, ref bool __result, Pawn ___pawn)
        {
            if (NeedsDefOf.O21Energy != null)
            {
                //Is the need our Energy need?
                if (nd == NeedsDefOf.O21Energy)
                {
                    if (___pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = true;
                    }
                    else
                    {
                        __result = false;
                    }
                }
            }
            //if (NeedsDefOf.O21Solar != null)
            //{
            //    if (nd == NeedsDefOf.O21Solar)
            //    {
            //        if (___pawn.def.HasModExtension<DefModExt_SolarNeed>())
            //        {
            //            __result = true;
            //        }
            //        else
            //        {
            //            __result = false;
            //        }
            //    }
            //}

            //if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest)
            //{
            //    if (___pawn.def.HasModExtension<ArtificialPawnProperties>())
            //    {
            //        ArtificialPawnProperties modExt = ___pawn.def.GetModExtension<ArtificialPawnProperties>();

            //        if (nd == NeedDefOf.Rest)
            //        {
            //            __result = modExt.needRest;
            //        }
            //        if (nd == NeedDefOf.Food)
            //        {
            //            __result = modExt.needFood;
            //        }
            //        else
            //        {
            //            __result = false;
            //        }
            //    }
            //}

            if (!___pawn.def.HasModExtension<EnergyHediffs>())
            {
                if (nd == NeedsDefOf.O21Energy)
                {
                    __result = false;
                }
            }
            //if (!___pawn.def.HasModExtension<DefModExt_SolarNeed>())
            //{
            //    if (nd == NeedsDefOf.O21Solar)
            //    {
            //        __result = false;
            //    }
            //}
        }
	}
}
