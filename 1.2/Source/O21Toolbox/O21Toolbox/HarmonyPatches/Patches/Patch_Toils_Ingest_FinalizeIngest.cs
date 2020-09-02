using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

namespace O21Toolbox.HarmonyPatches.Patches
{
    //[HarmonyPatch(typeof(Toils_Ingest), "FinalizeIngest")]
    //public static class Patch_Toils_Ingest_FinalizeIngest
    //{
    //    [HarmonyPostfix]
    //    static void Postfix(Toils_Ingest __instance, Pawn ingester, TargetIndex ingestibleInd)
    //    {
    //        Log.Message("Running Post Meal Delivery Service!");
    //        Thing food = ingester.jobs.curJob.GetTarget(ingestibleInd).Thing;
    //        if (food.def.HasModExtension<DefModExt_OutputFromEdible>())
    //        {
    //            DefModExt_OutputFromEdible ext = food.def.GetModExtension<DefModExt_OutputFromEdible>();
    //            if(ext.outputThing != null)
    //            {
    //                Thing output = GenSpawn.Spawn(ext.outputThing, ingester.Position, ingester.Map);
    //                //if (ingester.IsColonist)
    //                //{
    //                //    ingester.inventory.innerContainer.TryAddOrTransfer(output);
    //                //}
    //            }
    //        }
    //    }
    //}
}
