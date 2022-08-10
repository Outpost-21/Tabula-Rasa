using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils")]
    public class Patch_JobDriver_Vomit_MakeNewToils
    {
        [HarmonyPrefix]
        public static bool Prefix(ref JobDriver_Vomit __instance, ref IEnumerable<Toil> __result)
        {
            Pawn pawn = __instance.pawn;

            if (pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
            {
                JobDriver_Vomit instance = __instance;

                List<Toil> toils = new List<Toil>();
                toils.Add(new Toil()
                {
                    initAction = delegate ()
                    {
                        instance.pawn.jobs.StopAll();
                    }
                });

                __result = toils;
                return false;
            }

            return true;
        }

    }
}
