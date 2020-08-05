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
		// Token: 0x0600037B RID: 891 RVA: 0x00025AE4 File Offset: 0x00023CE4
		[HarmonyPostfix]
		public static void Postfix(NeedDef nd, ref bool __result, Pawn ___pawn)
        {
            NeedDef Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            NeedDef Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");

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

            if (!O21ToolboxSettings.Instance.EnergyNeedCompatMode)
            {
                if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest || nd == NeedDefOf.Joy || nd == NeedsDefOf.Beauty || nd == NeedsDefOf.Comfort || nd == NeedsDefOf.RoomSize || nd == NeedsDefOf.Outdoors || (Need_Bladder != null && nd == Need_Bladder) || (Need_Hygiene != null && nd == Need_Hygiene))
                {
                    if(___pawn.def.HasModExtension<EnergyHediffs>() && ___pawn.def.HasModExtension<ArtificialPawnProperties>())
                    {
                        ArtificialPawnProperties modExt = ___pawn.def.GetModExtension<ArtificialPawnProperties>();

                        if (nd == NeedDefOf.Rest)
                        {
                            __result = modExt.needRest;
                        }
                        if(nd == NeedDefOf.Food)
                        {
                            __result = modExt.needFood;
                        }
                        if (nd == NeedDefOf.Joy)
                        {
                            __result = modExt.needJoy;
                        }
                        if (nd == NeedsDefOf.Beauty)
                        {
                            __result = modExt.needBeauty;
                        }
                        if (nd == NeedsDefOf.Comfort)
                        {
                            __result = modExt.needComfort;
                        }
                        if (nd == NeedsDefOf.RoomSize)
                        {
                            __result = modExt.needRoomSize;
                        }
                        if (nd == NeedsDefOf.Outdoors)
                        {
                            __result = modExt.needOutdoors;
                        }
                        else
                        {
                            __result = false;
                        }
                    }
                    else if (___pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = false;
                    }
                }
            }

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
