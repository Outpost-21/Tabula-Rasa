using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class Verb_TerminusChain : Verb_LaunchProjectile
    {
        public VerbProps Props => verbProps as VerbProps;

        public override bool TryCastShot()
        {
            var chain = (TerminusChain) GenSpawn.Spawn(ThingDef.Named("TerminusChain"), currentTarget.Cell, caster.Map);
            chain.Target = caster;
            chain.Props = Props;
            chain.Projectile = Projectile;
            chain.Caster = caster;
            chain.Weapon = EquipmentSource;
            chain.FireAt(CurrentTarget.Thing);
            return true;
        }
    }

    [StaticConstructorOnStartup]
    public class TerminusChain : Thing
    {
        public Thing Caster;
        public Thing Holder;

        private int numBounces;
        private List<Thing> prevTargets = new List<Thing>();
        public ThingDef Projectile;

        public VerbProps Props;

        public Thing Target;
        private int ticksTillBounce;
        public Thing Weapon;

        public override void Draw()
        {
            var vec1 = Holder.DrawPos;
            var vec2 = Target.DrawPos;
            if (vec2.magnitude > vec1.magnitude)
            {
                var t = vec1;
                vec1 = vec2;
                vec2 = t;
            }

            Matrix4x4 matrix = default;
            matrix.SetTRS(vec2 + (vec1 - vec2) / 2, Quaternion.AngleAxis(vec1.AngleToFlat(vec2) + 90f, Vector3.up),
                new Vector3(1f,
                    1f, (vec1 - vec2).magnitude));
            Graphics.DrawMesh(MeshPool.plane10, matrix, Props.terminusChainGraphic.Graphic.MatSingle, 0);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref Target, "target");
            Scribe_References.Look(ref Holder, "holder");
            Scribe_Values.Look(ref ticksTillBounce, "ticksTillBounce");
            Scribe_Values.Look(ref numBounces, "numBounces");
            Scribe_Collections.Look(ref prevTargets, "prevTargets", LookMode.Reference);
            Scribe_Deep.Look(ref Props, "props");
        }

        public void FireAt(Thing target)
        {
            if (target == null || numBounces >= Props.bounceCount)
            {
                Destroy();
                return;
            }

            Holder = Target;
            prevTargets.Add(Target);
            Target = target;
            numBounces++;
            ticksTillBounce = Props.bounceDelay;
            var dinfo = new DamageInfo(Projectile.projectile.damageDef,
                Projectile.projectile.GetDamageAmount(Weapon?.def, Weapon?.Stuff) * Props.bounceDamageMultiplier,
                Projectile.projectile.GetArmorPenetration(Weapon), Holder.DrawPos.AngleToFlat(Target.DrawPos),
                Caster, null, Weapon?.def);
            var log = new BattleLogEntry_RangedImpact(Caster, Target, Target, Weapon?.def, Projectile, null);
            Find.BattleLog.Add(log);
            Target.TakeDamage(dinfo).AssociateWithLog(log);
            if (Props.impactRadius > 0f)
                GenExplosion.DoExplosion(Target.Position, Map, Props.impactRadius, Props.impactDamageDef,
                    Caster,
                    Mathf.RoundToInt(Props.impactDamageAmount * Props.bounceDamageMultiplier),
                    ignoredThings: new List<Thing> {this, Target});
        }

        public override void Tick()
        {
            if (ticksTillBounce > 0) ticksTillBounce--;

            if (ticksTillBounce <= 0) FireAt(NextTarget());

            Position = Holder.Position;
        }

        private Thing NextTarget()
        {
            var things = GenRadial.RadialDistinctThingsAround(Holder.Position, Map, Props.bounceRange, false)
                .Where(t => Props.targetParams.CanTarget(new TargetInfo(t)) &&
                            (Props.targetFriendly || t.HostileTo(Caster))).Except(new[] {this, Target});
            if (!Props.allowRepeat) things = things.Except(prevTargets);
            switch (Props.bouncePriority)
            {
                case BouncePriority.Near:
                    things = things.OrderBy(t => t.Position.DistanceTo(Holder.Position));
                    break;
                case BouncePriority.Far:
                    things = things.OrderByDescending(t => t.Position.DistanceTo(Holder.Position));
                    break;
                case BouncePriority.Random:
                    things = things.InRandomOrder();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return things.FirstOrDefault();
        }
    }
}