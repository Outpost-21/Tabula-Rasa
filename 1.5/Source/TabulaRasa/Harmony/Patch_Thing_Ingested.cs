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
    [HarmonyPatch(typeof(Thing), "Ingested")]
    public static class Patch_Thing_Ingested
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Thing __instance, Pawn ingester, float nutritionWanted)
        {
            DefModExt_OutputFromEdible modExt = __instance.def.GetModExtension<DefModExt_OutputFromEdible>();
            if (modExt != null)
            {
                if (modExt.geneRequired == null || ingester.genes.HasGene(modExt.geneRequired) && Rand.Chance(modExt.chance))
                {
                    Thing thing = ThingMaker.MakeThing(modExt.outputThing);
                    if(modExt.multiplier > 1)
                    {
                        thing.stackCount *= modExt.multiplier;
                    }
                    if (!GenPlace.TryPlaceThing(thing, ingester.Position, ingester.Map, ThingPlaceMode.Near))
                    {
                        Log.Error(string.Concat(new object[]
                        {
                                            ingester,
                                            " could not drop product ",
                                            thing,
                                            " near ",
                                            ingester.Position
                        }));
                    }
                }
            }
            return true;
        }
    }
}
