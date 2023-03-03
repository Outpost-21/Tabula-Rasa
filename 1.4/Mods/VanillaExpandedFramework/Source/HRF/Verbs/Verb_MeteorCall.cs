using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class Verb_MeteorCall : BaseVerb
    {
        public override bool TryCastShot()
        {
            var cell = CurrentTarget.Cell;
            if (verbProps.ForcedMissRadius > 0.5f)
            {
                var num = VerbUtility.CalculateAdjustedForcedMiss(verbProps.ForcedMissRadius,
                    currentTarget.Cell - caster.Position);
                if (num > 0.5f)
                {
                    var num2 = Rand.Range(0, GenRadial.NumCellsInRadius(num));
                    if (num2 > 0) cell += GenRadial.RadialPattern[num2];
                }
            }

            var meteor = (MeteorIncoming) SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("MeteorIncoming"), cell,
                caster.Map);
            meteor.Props = Props;
            meteor.def.skyfaller.explosionDamage = Props.meteorDamageDef;
            meteor.def.skyfaller.explosionDamageFactor = Props.meteorDamageDef.defaultDamage / Props.meteorDamageAmount;
            return true;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            return Props.meteorSize;
        }
    }

    public class MeteorIncoming : Skyfaller
    {
        public VerbProps Props;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref Props, "props");
        }

        public override void SpawnThings()
        {
            if (Props.spawnRocks)
                foreach (var c in GenRadial.RadialCellsAround(Position, Props.meteorSize + 1, true))
                {
                    if (!Rand.Chance((Props.meteorSize - 1) / c.DistanceTo(Position))) continue;
                    SpawnThingAt(c);
                }
            else
                foreach (var pawn in GenRadial.RadialDistinctThingsAround(Position, Map, Props.meteorSize, true)
                    .OfType<Pawn>())
                {
                    var theta = Position.ToVector3().AngleToFlat(pawn.Position.ToVector3());
                    var radius = Mathf.CeilToInt(Props.meteorSize + 1);
                    pawn.Position = Position + IntVec3.FromVector3(new Vector3(radius, 0, 0).RotatedBy(theta));
                    pawn.stances.stunner.StunFor(60, this);
                    pawn.Notify_Teleported(false, false);
                }
        }

        private void SpawnThingAt(IntVec3 c)
        {
            var thingDef = Props.meteorMaterial.RandomElement();
            var thing = ThingMaker.MakeThing(thingDef);
            foreach (var t in c.GetThingList(Map).ListFullCopy().Where(t => !(t is Pawn)).Where(t =>
                t.def.Fillage != FillCategory.None && thingDef.Fillage == FillCategory.Full)) t.Destroy();

            GenSpawn.Spawn(thing, c, Map);

            if (Props.allowCrushingRocks)
                PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);

            if (thing.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion &&
                def.skyfaller.explosionDamage.isExplosive &&
                thing.Position.InHorDistOf(Position, def.skyfaller.explosionRadius))
                Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
        }
    }
}