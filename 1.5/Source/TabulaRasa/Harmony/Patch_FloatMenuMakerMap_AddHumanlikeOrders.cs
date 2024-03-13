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
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class Patch_FloatMenuMakerMap_AddHumanlikeOrders
    {
        [HarmonyPostfix]
        public static void PostFix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            var c = IntVec3.FromVector3(clickPos);
            HumanlikeOrdersUtil.optsID = "";
            HumanlikeOrdersUtil.optsID += pawn.ThingID;
            HumanlikeOrdersUtil.optsID += c.ToString();
            if (opts != null)
            {
                for (var i = 0; i < opts.Count; i++)
                {
                    HumanlikeOrdersUtil.optsID += opts[i].Label;
                }
            }
            if (HumanlikeOrdersUtil.optsID == HumanlikeOrdersUtil.lastOptsID)
            {
                opts?.AddRange(HumanlikeOrdersUtil.savedList);
                return;
            }
            LogUtil.LogDebug("Humanlike Orders :: New list constructed");
            LogUtil.LogDebug($"Humanlike Orders :: {HumanlikeOrdersUtil.optsID}");
            HumanlikeOrdersUtil.lastOptsID = HumanlikeOrdersUtil.optsID;
            HumanlikeOrdersUtil.savedList.Clear();
            var things = c.GetThingList(pawn.Map);
            if (things.Count > 0)
            {
                foreach (var pair in HumanlikeOrdersUtil.FloatMenuOptionList)
                {
                    if (!pair.Value.NullOrEmpty())
                    {
                        var passers = things.FindAll(x => pair.Key.Passes(x));
                        foreach (var passer in passers)
                        {
                            foreach (var func in pair.Value)
                            {
                                if (func.Invoke(clickPos, pawn, passer) is List<FloatMenuOption> newOpts && newOpts.Count > 0)
                                {
                                    opts?.AddRange(newOpts);
                                    HumanlikeOrdersUtil.savedList.AddRange(newOpts);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
