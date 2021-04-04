using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.BuildingExt;

namespace O21Toolbox.HarmonyPatches
{

    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[] 
    { 
        typeof(Vector3), 
        typeof(float), 
        typeof(bool), 
        typeof(Rot4), 
        typeof(Rot4), 
        typeof(RotDrawMode), 
        typeof(bool), 
        typeof(bool), 
        typeof(bool) 
    })]
    class Patch_PawnRenderer_RenderPawnInternal
    {
        [HarmonyPrefix]
        static bool Prefix(bool portrait, Pawn ___pawn)
        {
            if (portrait || ___pawn?.Map == null || ___pawn?.RaceProps?.Humanlike != true)
            {
                return true;
            }
            return !(___pawn?.CurrentBed()?.def?.HasModExtension<DefModExt_BedExtensions>() == true);
        }
    }

    [HarmonyPatch(typeof(Pawn_MindState), "MindStateTick")]
    public class Patch_Pawn_MindState_MindStateTick
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_MindState __instance)
        {
            if (Find.TickManager.TicksGame % 123 == 0 && __instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null)
            {
                Building_Bed currBed = __instance.pawn.CurrentBed();
                if (currBed == null) 
                { 
                    return; 
                }

                DefModExt_BedExtensions modExt = currBed.def.GetModExtension<DefModExt_BedExtensions>();
                if (modExt == null || !modExt.ignoreRain) 
                { 
                    return; 
                }

                WeatherDef curWeatherLerped = __instance.pawn.Map.weatherManager.CurWeatherLerped;
                if (curWeatherLerped.exposedThought != null && curWeatherLerped.exposedThought == ThoughtDef.Named("SoakingWet") && !__instance.pawn.Position.Roofed(__instance.pawn.Map))
                {
                    __instance.pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(curWeatherLerped.exposedThought);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Toils_LayDown))]
    class Patch_Toils_LayDown
    {
        [HarmonyPostfix]
        [HarmonyPatch("ApplyBedThoughts")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        static void PostFix(Pawn actor)
        {
            Building_Bed building = actor.CurrentBed();
            if (building != null)
            {
                DefModExt_BedExtensions modExt = building.def.GetModExtension<DefModExt_BedExtensions>();
                if (modExt != null)
                {
                    CompPowerTrader compPower = building.TryGetComp<CompPowerTrader>();
                    if(compPower == null && (modExt.coldPowered || modExt.heatPowered))
                    {
                        Log.Error(building.def.defName + " is set to require power for BedExtensions but does not have a power comp.");
                    }
                    if (modExt.ignoreOutdoors)
                    {
                        actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside);
                    }
                    if (modExt.ignoreCold && (!modExt.coldPowered || compPower.PowerOn))
                    {
                        actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold);
                    }
                    if (modExt.ignoreHeat && (!modExt.heatPowered || compPower.PowerOn))
                    {
                        actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat);
                    }
                    if (modExt.ignoreOthers)
                    {
                        actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBarracks);
                    }
                }
            }


        }

    }
}
