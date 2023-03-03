using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_SpawnFaction : BaseVerb
    {
        public override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map) return false;
            var thing = ThingMaker.MakeThing(verbProps.spawnDef);
            thing.SetFaction(Caster.Faction);
            GenPlace.TryPlaceThing(thing, currentTarget.Cell, caster.Map, ThingPlaceMode.Near);
            if (verbProps.colonyWideTaleDef != null)
            {
                var pawn = caster.Map.mapPawns.FreeColonistsSpawned.RandomElementWithFallback();
                TaleRecorder.RecordTale(verbProps.colonyWideTaleDef, caster, pawn);
            }

            ReloadableCompSource?.UsedOnce();
            return true;
        }
    }
}