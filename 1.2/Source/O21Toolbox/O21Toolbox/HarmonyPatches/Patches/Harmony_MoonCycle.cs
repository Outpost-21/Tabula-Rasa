using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.MoonCycle;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(TickManager), nameof(TickManager.DebugSetTicksGame))]
    public static class Patch_TickManager_DebugSetTicksGame
    {
        [HarmonyPostfix]
        public static void MoonTicksUpdate(TickManager __instance, int newTicksGame)
        {
            if (newTicksGame <= Find.TickManager.TicksGame + GenDate.TicksPerDay + 1000)
            {
                Find.World.GetComponent<WorldComponent_MoonCycle>().AdvanceOneDay();
            }
            else if (newTicksGame <= Find.TickManager.TicksGame + GenDate.TicksPerQuadrum + 1000)
            {
                Find.World.GetComponent<WorldComponent_MoonCycle>().AdvanceOneQuadrum();
            }
        }
    }
}
