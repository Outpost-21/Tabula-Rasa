using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    /// <summary>
    /// Utilities for getting butcher drops.
    /// </summary>
    public static class ButcherUtility
    {
        /// <summary>
        /// Spawn drops from butchering.
        /// </summary>
        public static void SpawnDrops(Pawn pawn, IntVec3 position, Map map)
        {
            //Get amount of body remaining.
            float remainingBody = pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(pawn.RaceProps.body.corePart);

            //Spawn butcher products.
            foreach (ThingDefCountClass butcherThingCount in pawn.def.butcherProducts)
            {
                int finalThingCount = (int)(Math.Ceiling(butcherThingCount.count * remainingBody));

                if (finalThingCount > 0)
                    do
                    {
                        Thing product = ThingMaker.MakeThing(butcherThingCount.thingDef);
                        product.stackCount = Math.Min(finalThingCount, butcherThingCount.thingDef.stackLimit);

                        finalThingCount -= product.stackCount;

                        GenPlace.TryPlaceThing(product, position, map, ThingPlaceMode.Near);
                    } while (finalThingCount > 0);
            }
        }
    }
}
