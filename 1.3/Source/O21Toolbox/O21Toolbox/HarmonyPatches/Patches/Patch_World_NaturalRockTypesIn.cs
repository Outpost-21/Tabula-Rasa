using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

using HarmonyLib;

using O21Toolbox.BiomeExt;

namespace O21Toolbox
{
    [HarmonyPatch(typeof(World))]
    [HarmonyPatch("NaturalRockTypesIn")]
    public static class Patch_World_NaturalRockTypesIn
    {
        [HarmonyPostfix]
        public static void MakeRocksAccordingToBiome(int tile, ref World __instance, ref IEnumerable<ThingDef> __result)
        {
            if (!__instance.grid.tiles[tile].biome.HasModExtension<DefModExt_StoneTypeOverride>())
            {
                return;
            }
            DefModExt_StoneTypeOverride modExt = __instance.grid.tiles[tile].biome.GetModExtension<DefModExt_StoneTypeOverride>();
            List<ThingDef> newStones = new List<ThingDef>();
            if (!modExt.removeOriginals)
            {
                List<ThingDef> oldStones = __result.ToList();
                for (int i = 0; i < oldStones.Count(); i++)
                {
                    newStones.Add(oldStones[i]);
                }
            }
            if (!modExt.thingDefs.NullOrEmpty())
            {
                for (int i = 0; i < modExt.thingDefs.Count(); i++)
                {
                    newStones.Add(modExt.thingDefs[i]);
                }
            }
            if (newStones.NullOrEmpty())
            {
                Log.Error("Biome Stones were about to set to null, biome def: " + __instance.grid.tiles[tile].biome.defName + " must be configured improperly.");
                return;
            }

            __result = newStones.AsEnumerable();
        }
    }
}
