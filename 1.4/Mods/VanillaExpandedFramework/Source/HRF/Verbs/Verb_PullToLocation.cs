using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class Verb_PullToLocation : BaseVerb
    {
        public override bool TryCastShot()
        {
            if (CurrentTarget.Cell.GetFirstThing<Puller>(caster.Map) != null) return false;
            var puller = (Puller) GenSpawn.Spawn(ThingDef.Named("Puller"), CurrentTarget.Cell, caster.Map);
            puller.Caster = caster;
            puller.Weapon = EquipmentSource?.def;
            puller.Props = Props;
            return true;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            return Props.effectRadius;
        }
    }

    public class Puller : Thing
    {
        public Thing Caster;
        private int lastDamageTick = Find.TickManager.TicksGame;
        private List<PawnMover> movers = new List<PawnMover>();
        public VerbProps Props;
        private int startTick = Find.TickManager.TicksGame;
        public ThingDef Weapon;

        public override void Tick()
        {
            if (lastDamageTick + Props.effectRate <= Find.TickManager.TicksGame)
            {
                movers.RemoveAll(mover => mover.FlyingPawn == null);
                foreach (var pawn in GenRadial.RadialDistinctThingsAround(Position, Map, Props.effectRadius, false)
                    .OfType<Pawn>().Except(movers.Select(mover => mover.FlyingPawn)).Except(Caster as Pawn))
                {
                    DamagePawn(pawn);
                    switch (Props.effectBehavior)
                    {
                        case Behavior.Pull:
                            pawn.Position += new IntVec3(1, 0, 0).RotatedBy(Rot4.FromAngleFlat(pawn.Position.ToVector3()
                                .AngleToFlat(Position.ToVector3())));
                            pawn.Notify_Teleported(false, false);
                            break;
                        case Behavior.Spin:
                            var spinner =
                                (PawnSpinner) PawnFlyer.MakeFlyer(ThingDef.Named("SpinnedPawn"), pawn, Position, null, null);
                            spinner.DurationTicks = startTick + Props.effectDuration - Find.TickManager.TicksGame;
                            spinner.RotationPerTick = Props.rotationSpeed / 60f;
                            spinner.Center = DrawPos;
                            spinner.InwardMotionPerTick = Props.inwardSpeed / 60f;
                            GenSpawn.Spawn(spinner, Position, Map);
                            movers.Add(spinner);
                            break;
                        case Behavior.Hold:
                            pawn.pather.StopDead();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (Props.effectBehavior == Behavior.Spin)
                    movers.ForEach(mover =>
                    {
                        if (mover.FlyingPawn != null) DamagePawn(mover.FlyingPawn);
                    });

                lastDamageTick = Find.TickManager.TicksGame;
            }

            if (startTick + Props.effectDuration <= Find.TickManager.TicksGame)
            {
                movers.ForEach(mover => mover.End());
                movers.Clear();
                Destroy();
            }
        }

        private void DamagePawn(Pawn pawn)
        {
            var dinfo = new DamageInfo(Props.effectDamageDef, Props.effectDamageAmount,
                Props.effectDamageDef.defaultArmorPenetration, DrawPos.AngleToFlat(pawn.DrawPos), Caster, null,
                Weapon);
            var log = new BattleLogEntry_RangedImpact(Caster, pawn, pawn, Weapon, def,
                null);
            Find.BattleLog.Add(log);
            pawn.TakeDamage(dinfo).AssociateWithLog(log);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref Caster, "caster");
            Scribe_Deep.Look(ref Props, "props");
            Scribe_Defs.Look(ref Weapon, "weapon");
            Scribe_Collections.Look(ref movers, "movers", LookMode.Reference);
            Scribe_Values.Look(ref startTick, "startTick");
            Scribe_Values.Look(ref lastDamageTick, "lastDamageTick");
        }
    }

    public class PawnSpinner : PawnMover
    {
        public Vector3 Center;
        public int DurationTicks;
        private float initialAngle;
        public float InwardMotionPerTick;
        public float RotationPerTick;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (FlyingPawn != null) initialAngle = Center.AngleToFlat(startVec);
        }

        protected override void RecomputePosition()
        {
            if (CheckRecompute()) return;
            var dist = Mathf.Max(0.5f, Mathf.Abs((startVec - Center).magnitude) - InwardMotionPerTick * ticksFlying);
            var angle = initialAngle + ticksFlying * RotationPerTick;
            effectivePos = Center + new Vector3(dist, 0, 0).RotatedBy(angle) + Altitudes.AltIncVect;
            Position = FlyingPawn.Position = IntVec3.FromVector3(effectivePos).ClampInsideMap(Map);
        }

        public override void Tick()
        {
            base.Tick();
            ticksFlightTime = int.MaxValue;
            DurationTicks--;
            if (DurationTicks <= 0) End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DurationTicks, "duration");
            Scribe_Values.Look(ref RotationPerTick, "rotation");
            Scribe_Values.Look(ref Center, "center");
        }
    }
}