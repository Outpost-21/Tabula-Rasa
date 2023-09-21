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
    public class Comp_ClusterGrower : ThingComp
    {
        public CompProperties_ClusterGrower Props => (CompProperties_ClusterGrower)props;

        public int lastGrowthTick = -1;

        public int lastUndergrowthTick = -1;

        public bool undergrowthDone = false;

        public bool showDebugRendering = false;

        public List<IntVec3> undergrowthCellsRemaining = new List<IntVec3>();

        public List<IntVec3> currentViableUndergrowthCells => undergrowthCellsRemaining.Where(c => HasAdjacentUndergrowth(c, parent.Map) && IsValidUndergrowthTarget(c, parent.Map)).ToList();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Plant plant = parent as Plant;
                if (plant != null)
                {
                    if (Props.undergrowth != null)
                    {
                        undergrowthCellsRemaining = GenRadial.RadialCellsAround(parent.Position, Props.undergrowthRadius, false).ToList();
                        if (parent.Position.GetFirstThing(parent.Map, Props.undergrowth) == null)
                        {
                            GenerateUndergrowth(parent.Position, parent.Map, true);
                        }
                    }
                    if (!plant.sown && plant.Growth > 0.2f)
                    {
                        if (Props.undergrowth != null)
                        {
                            while (AttemptUndergrowth()) { }
                        }

                        while (AttemptGrowth(true)) { }
                    }
                }
                lastGrowthTick = Find.TickManager.TicksGame;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            Tick();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            Tick();
        }

        public override void CompTickLong()
        {
            base.CompTickLong();
            Tick();
        }

        public void Tick()
        {
            if (Find.TickManager.TicksGame > lastGrowthTick + Props.growthTicks)
            {
                AttemptGrowth();
                lastGrowthTick = Find.TickManager.TicksGame;
            }
            if (Props.undergrowth != null && Find.TickManager.TicksGame > lastUndergrowthTick + Props.undergrowthTicks)
            {
                if (!undergrowthCellsRemaining.NullOrEmpty())
                {
                    AttemptUndergrowth();
                }
                lastUndergrowthTick = Find.TickManager.TicksGame;
            }
        }

        public bool AttemptGrowth(bool randomGrowth = false)
        {
            bool success = false;
            foreach (ClusterPlantClass clusterPlant in Props.clusterPlants)
            {
                Plant plant = parent as Plant;
                if(clusterPlant.matureOnly && !plant.HarvestableNow)
                {
                    continue;
                }
                if (CountThingsInRadius(clusterPlant.def, parent.Position, parent.Map, clusterPlant.radius.max) <= clusterPlant.count)
                {
                    if (TryGrowThingInRadius(clusterPlant.def, parent.Position, parent.Map, clusterPlant.radius, clusterPlant.onUndergrowthOnly, randomGrowth, clusterPlant.minDistance))
                    {
                        success = true;
                    }
                }
            }
            return success;
        }

        public bool AttemptUndergrowth()
        {
            bool success = false;
            IntVec3 attemptLoc = currentViableUndergrowthCells.RandomElementByWeightWithFallback(c => Props.undergrowthCurve.Evaluate(c.DistanceTo(parent.Position)), default(IntVec3));
            if (attemptLoc != null && attemptLoc != default(IntVec3))
            {
                GenerateUndergrowth(attemptLoc, parent.Map);
            }
            undergrowthCellsRemaining.Remove(attemptLoc);
            return success;
        }

        public void GenerateUndergrowth(IntVec3 c, Map map, bool forced = false)
        {
            if (forced || Rand.Chance(Props.undergrowthCurve.Evaluate(c.DistanceTo(parent.Position))))
            {
                if (Props.undergrowthClears && c != parent.Position)
                {
                    Plant plant = c.GetPlant(map);
                    if (plant != null)
                    {
                        plant.Destroy();
                    }
                }
                GenSpawn.Spawn(Props.undergrowth, c, map);
            }
        }

        public bool HasAdjacentUndergrowth(IntVec3 c, Map map)
        {
            if(!c.IsValid || !c.InBounds(map)) { return false; }
            List<IntVec3> adjacentCells = GenAdjFast.AdjacentCells8Way(c);
            foreach (IntVec3 cell in adjacentCells)
            {
                if (cell.GetFirstThing(map, Props.undergrowth) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidUndergrowthTarget(IntVec3 c, Map map)
        {
            if(!c.IsValid || !c.InBounds(map)) 
            { 
                return false; 
            }
            if(c.GetFirstThing(map, Props.undergrowth) != null)
            {
                return false;
            }
            if(c.GetFirstBuilding(map) != null)
            {
                return false;
            }
            return true;
        }

        public bool TryGrowThingInRadius(ThingDef targetDef, IntVec3 center, Map map, FloatRange radiusRange, bool onUndergrowthOnly = false, bool randomGrowth = false, float minDistance = 0f)
        {
            List<IntVec3> invalidCells = GenRadial.RadialCellsAround(center, radiusRange.min, false).ToList();
            List<IntVec3> validCells = GenRadial.RadialCellsAround(center, radiusRange.max, false).ToList();
            validCells.RemoveAll(c => invalidCells.Contains(c));
            List<IntVec3> potentialCells = validCells?.Where(c => ValidGrowthSpot(targetDef, c, map, onUndergrowthOnly, minDistance))?.ToList();
            if (!potentialCells.NullOrEmpty())
            {
                Plant plant = GenSpawn.Spawn(targetDef, potentialCells.RandomElement(), map, WipeMode.Vanish) as Plant;
                if (randomGrowth)
                {
                    plant.Growth = Rand.Range(0f, 1f);
                }
                return true;
            }
            return false;
        }

        public bool ValidGrowthSpot(ThingDef targetDef, IntVec3 c, Map map, bool undergrowthOnly, float minDistance)
        {
            if (!c.InBounds(map))
            {
                return false;
            }
            if (c.GetThingList(map).Any(t => t as Plant != null))
            {
                return false;
            }
            if (undergrowthOnly && c.GetFirstThing(map, Props.undergrowth) == null)
            {
                return false;
            }
            if(c.GetThingList(map).Any(t => Props.cannotGrowOver.Contains(t.def)))
            {
                return false;
            }
            if (CountThingsInRadius(targetDef, c, map, minDistance) > 0)
            {
                return false;
            }
            return true;
        }

        public int CountThingsInRadius(ThingDef targetDef, IntVec3 center, Map map, float radius)
        {
            int count = 0;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, radius, true))
            {
                if (cell.InBounds(map))
                {
                    Thing thing = cell.GetFirstThing(map, targetDef);
                    if (thing != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref lastGrowthTick, "lastGrowthTick", -1);
            Scribe_Values.Look(ref lastUndergrowthTick, "lastUndergrowthTick", -1);
            Scribe_Values.Look(ref undergrowthDone, "undergrowthDone", false);
            Scribe_Collections.Look(ref undergrowthCellsRemaining, "undergrowthCellsRemaining");
        }

        public override void PostDraw()
        {
            if (showDebugRendering)
            {
                GenDraw.DrawFieldEdges(undergrowthCellsRemaining, Color.white);
                GenDraw.DrawFieldEdges(currentViableUndergrowthCells, Color.red);
            }
            base.PostDraw();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            yield return new Command_Action()
            {
                defaultLabel = "DEV: Toggle Debug Info",
                defaultDesc = "Show debug information.",
                action = delegate ()
                {
                    showDebugRendering = !showDebugRendering;
                }
            };
        }
    }
}
