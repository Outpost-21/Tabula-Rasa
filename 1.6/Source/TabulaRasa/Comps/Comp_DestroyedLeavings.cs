using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Comp_DestroyedLeavings : ThingComp
    {
        public CompProperties_DestroyedLeavings Props => (CompProperties_DestroyedLeavings)props;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (Rand.Chance(Props.chance))
            {
                float percentage = Props.percentRange.RandomInRange;
                foreach(ThingDefCount c in Props.leavings)
                {
                    if(c != null)
                    {
                        Thing thing = ThingMaker.MakeThing(c.ThingDef);
                        thing.stackCount = Mathf.CeilToInt(c.Count * percentage);
                        QualityCategory quality;
                        CompQuality qualComp = thing.TryGetComp<CompQuality>();
                        if (parent.TryGetQuality(out quality) && qualComp != null)
                        {
                            qualComp.SetQuality(quality, ArtGenerationContext.Outsider);
                        }
                        if (thing.def.Minifiable)
                        {
                            thing = thing.MakeMinified();
                        }
                        if (parent is Plant plant)
                        {
                            if (!Props.harvestableOnly || plant.HarvestableNow)
                            {
                                SpawnThing(thing);
                            }
                        }
                        else { SpawnThing(thing); }
                    }
                }
            }
            base.PostDestroy(mode, previousMap);
        }

        public void SpawnThing(Thing thing)
        {
            GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Direct);
        }
    }
}
