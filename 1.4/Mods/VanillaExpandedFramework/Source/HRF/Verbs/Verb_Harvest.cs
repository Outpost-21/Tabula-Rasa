using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace HRF
{
    public class Verb_Harvest : Verb_AreaEffect
    {
        protected override void AffectCell(IntVec3 cell)
        {
            var plants = cell.GetThingList(caster.Map).OfType<Plant>().Where(p =>
                p.def.plant.harvestedThingDef != null && p.HarvestableNow).ToList();
            foreach (var plant in plants)
            {
                if (!Props.canHarvestTrees && plant.def.defName.Contains("Tree")) continue;

                var thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef);
                thing.stackCount = (int) (plant.YieldNow() * Props.yieldMultiplier);

                if (thing.stackCount <= 0) continue;
                if (caster.Faction != Faction.OfPlayer) thing.SetForbidden(true);

                Find.QuestManager.Notify_PlantHarvested(CasterPawn, thing);
                CasterPawn?.records?.Increment(RecordDefOf.PlantsHarvested);
                GenPlace.TryPlaceThing(thing, plant.Position, caster.Map, ThingPlaceMode.Near);
                plant.def.plant.soundHarvestFinish?.PlayOneShot(caster);
                plant.PlantCollected(CasterPawn, PlantDestructionMode.Cut);
            }
        }
    }
}