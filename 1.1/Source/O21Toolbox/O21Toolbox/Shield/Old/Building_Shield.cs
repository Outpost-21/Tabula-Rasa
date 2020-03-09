using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace O21Toolbox.Shield
{
    [StaticConstructorOnStartup]
    public class Building_Shield : Building, IAttackTarget, ILoadReferenceable
    {
        private static readonly Material BaseBubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        private float curShieldStress = 0f;

        private int lastAbsorbDamageTick;

        private int shieldScaleX = 10;
        private int shieldScaleY;

        private int ticksToReset;

        public bool active;

        private bool checkedPowerComp = false;
        private CompPowerTrader cachedPowerComp;

        public HashSet<IntVec3> coveredCells;
        public HashSet<IntVec3> scanCells;

        private int shieldBuffer = 0;
        private Vector3 impactAngleVect;

        private List<Thing> affectedThingsKeysWorkingList;
        private List<int> affectedThingsValuesWorkingList;
        public Dictionary<Thing, int> affectedThings = new Dictionary<Thing, int>();

        private const int CacheUpdateInterval = 10;
        private const float EdgeCellRadius = 5f;

        public DefModExt_ShieldProperties ShieldModExt => def.GetModExtension<DefModExt_ShieldProperties>() ?? DefModExt_ShieldProperties.defaultValues;

        private CompPowerTrader powerTrader
        {
            get
            {
                if (!checkedPowerComp)
                {
                    cachedPowerComp = GetComp<CompPowerTrader>();
                    checkedPowerComp = true;
                }
                return cachedPowerComp;
            }
        }

        public float MaxShieldStress => 1.0f;

        public float CurShieldStress
        {
            get
            {
                return curShieldStress;
            }
            set
            {
                curShieldStress = Mathf.Clamp(value, 0f, 1f);
                if (curShieldStress >= 1f)
                {
                    this.Notify_ShieldCollapse();
                }
            }
        }

        public Vector2 ShieldScale => new Vector2(shieldScaleX, shieldScaleY);

        public float TotalVentTemperature => 0;

        public float StressReductionPerTick => (0.03f * TotalVentTemperature);

        public IEnumerable<Thing> ThingsInRadius
        {
            get
            {
                foreach(IntVec3 cell in coveredCells)
                {
                    foreach(Thing thing in cell.GetThingList(MapHeld))
                    {
                        yield return thing;
                    }
                }
                yield break;

            }
        }

        public IEnumerable<Thing> ThingsInScanArea
        {
            get
            {
                foreach (IntVec3 cell in this.scanCells)
                {
                    foreach (Thing thing in cell.GetThingList(base.MapHeld))
                    {
                        yield return thing;
                    }
                }
                yield break;
            }
        }

        public Thing Thing
        {
            get
            {
                return this;
            }
        }

        public LocalTargetInfo TargetCurrentlyAimingAt
        {
            get
            {
                return LocalTargetInfo.Invalid;
            }
        }

        public float TargetPriorityFactor
        {
            get
            {
                return 1f;
            }
        }

        private bool CanFunction
        {
            get
            {
                return (this.powerTrader == null || this.powerTrader.PowerOn) && !this.IsBrokenDown();
            }
        }

        private void Notify_ShieldCollapse()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            MoteMaker.MakeStaticMote(this.TrueCenter(), base.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = this.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, base.Map, Rand.Range(0.8f, 1.2f));
            }
            this.ticksToReset = this.ShieldModExt.resetTime;
        }
        public bool WithinBoundary(IntVec3 sourcePos, IntVec3 checkedPos)
        {
            return (this.coveredCells.Contains(sourcePos) && this.coveredCells.Contains(checkedPos)) || (!this.coveredCells.Contains(sourcePos) && !this.coveredCells.Contains(checkedPos));
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            map.GetComponent<MapComp_ShieldList>().shieldGenList.Add(this);
            UpdateCoveredCells();
        }

        public void UpdateCoveredCells()
        {
            coveredCells = new HashSet<IntVec3>(GenRadial.RadialCellsAround(PositionHeld, shieldScaleX, true));
            bool flag = shieldScaleX < 6f;
            if (flag)
            {
                scanCells = coveredCells;
            }
            else
            {
                IEnumerable<IntVec3> interiorCells = GenRadial.RadialCellsAround(PositionHeld, shieldScaleX - 5f, true);
                scanCells = new HashSet<IntVec3>(from c in coveredCells
                                                      where !interiorCells.Contains(c)
                                                      select c);
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map.GetComponent<MapComp_ShieldList>().shieldGenList.Remove(this);
            coveredCells = null;
            scanCells = null;
            base.DeSpawn(mode);
        }
        public override void Tick()
        {
            bool flag = this.IsHashIntervalTick(15);
            if (flag)
            {
                this.UpdateCache();
            }
            bool canFunction = this.CanFunction;
            if (canFunction)
            {
                if (ticksToReset > 0)
                {
                    ticksToReset--;
                    if (ticksToReset <= 0)
                    {
                        CurShieldStress = 0f;
                    }
                }
                else
                {
                    CurShieldStress += StressReductionPerTick;
                }
                bool flag4 = active;
                if (flag4)
                {
                    bool flag5 = powerTrader != null;
                    if (flag5)
                    {
                        powerTrader.PowerOutput = -powerTrader.Props.basePowerConsumption;
                    }
                    bool flag6 = (shieldScaleX < 6f || Find.TickManager.TicksGame % 2 == 0) && CurShieldStress < 1f;
                    if (flag6)
                    {
                        ShieldTick();
                    }
                }
                else
                {
                    bool flag7 = this.powerTrader != null;
                    if (flag7)
                    {
                        powerTrader.PowerOutput = -(ShieldModExt.powerUsageBase * ShieldModExt.powerUsageFactorPassive);
                    }
                }
            }
            base.Tick();
        }
        private void UpdateCache()
        {
            for (int i = 0; i < this.affectedThings.Count; i++)
            {
                Thing thing = this.affectedThings.Keys.ToList<Thing>()[i];
                bool flag = this.affectedThings[thing] <= 0;
                if (flag)
                {
                    this.affectedThings.Remove(thing);
                }
                else
                {
                    Dictionary<Thing, int> dictionary = this.affectedThings;
                    Thing key = thing;
                    dictionary[key] -= 15;
                }
            }
            this.active = (base.ParentHolder is Map && this.CanFunction && (GenHostility.AnyHostileActiveThreatTo(base.MapHeld, base.Faction, false) || base.Map.listerThings.ThingsOfDef(ThingDefOf.Tornado).Any<Thing>() || base.Map.listerThings.ThingsOfDef(ThingDefOf.DropPodIncoming).Any<Thing>() || this.shieldBuffer > 0));
            bool flag2 = (GenHostility.AnyHostileActiveThreatTo(base.MapHeld, base.Faction, false) || base.Map.listerThings.ThingsOfDef(ThingDefOf.Tornado).Any<Thing>() || base.Map.listerThings.ThingsOfDef(ThingDefOf.DropPodIncoming).Any<Thing>()) && this.shieldBuffer < 10;
            if (flag2)
            {
                this.shieldBuffer = 10;
            }
            else
            {
                this.shieldBuffer--;
            }
        }
        private void ShieldTick()
        {
            HashSet<Thing> hashSet = new HashSet<Thing>(this.ThingsInRadius);
            HashSet<Thing> hashSet2 = new HashSet<Thing>(this.ThingsInScanArea);
            foreach (Thing thing in hashSet2)
            {
                Projectile projectile = thing as Projectile;
                bool flag = projectile != null && projectile.BlockableByShield(this);
                if (flag)
                {
                    Thing thing2 = NonPublicFields.Projectile_launcher.GetValue(projectile) as Thing;
                    bool flag2 = thing2 != null && !hashSet.Contains(thing2);
                    if (flag2)
                    {
                        bool flag3 = !(projectile is Projectile_Explosive);
                        if (flag3)
                        {
                            this.AbsorbDamage((float)projectile.DamageAmount, projectile.def.projectile.damageDef, projectile.ExactRotation.eulerAngles.y);
                        }
                        projectile.Position += Rot4.FromAngleFlat((base.Position - projectile.Position).AngleFlat).Opposite.FacingCell;
                        NonPublicFields.Projectile_usedTarget.SetValue(projectile, new LocalTargetInfo(projectile.Position));
                        NonPublicMethods.Projectile_ImpactSomething(projectile);
                    }
                }
                Skyfaller skyfaller = thing as Skyfaller;
                bool flag4 = skyfaller != null;
                if (flag4)
                {
                }
            }
        }
        private float EnergyLossMultiplier(DamageDef damageDef)
        {
            bool flag = damageDef == DamageDefOf.EMP;
            float result;
            if (flag)
            {
                result = 4f;
            }
            else
            {
                result = 1f;
            }
            return result;
        }
        public void AbsorbDamage(float amount, DamageDef def, Thing source)
        {
            this.AbsorbDamage(amount, def, (this.TrueCenter() - source.TrueCenter()).AngleFlat());
        }
        public void AbsorbDamage(float amount, DamageDef def, float angle)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(angle);
            Vector3 loc = this.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * (this.shieldScaleX / 2f);
            float num = Mathf.Min(10f, 2f + amount / 10f);
            MoteMaker.MakeStaticMote(loc, base.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, base.Map, Rand.Range(0.8f, 1.2f));
            }
            CurShieldStress -= amount * EnergyLossMultiplier(def) * 0.033f;
            if (CurShieldStress > ShieldModExt.shieldOverloadThreshold && Rand.Chance(ShieldModExt.shieldOverloadChance * (1f - CurShieldStress)))
            {
                GenExplosion.DoExplosion(this.OccupiedRect().RandomCell, Map, 1.9f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
        }
        public override void Draw()
        {
            base.Draw();
            bool flag = active && CurShieldStress < 1f;
            if (flag)
            {
                float num = shieldScaleX * 2f * Mathf.Lerp(0.9f, 1.1f, CurShieldStress / MaxShieldStress);
                Vector3 vector = DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                bool flag2 = num2 < 8;
                if (flag2)
                {
                    float num3 = (8 - num2) / 8f * 0.05f;
                    vector += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 45);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default;
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Material material = new Material(BaseBubbleMat);
                material.color = ShieldModExt.shieldColour;
                Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
            }
        }
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (dinfo.Def == DamageDefOf.EMP)
            {
                CurShieldStress += dinfo.Amount * 0.1f;
            }
            base.PreApplyDamage(ref dinfo, out absorbed);
        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = !active;
            if (flag)
            {
                stringBuilder.AppendLine("InactiveFacility".Translate().CapitalizeFirst());
            }
            stringBuilder.AppendLine(base.GetInspectString());
            return stringBuilder.ToString().TrimEndNewlines();
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            bool flag = Find.Selector.SingleSelectedThing == this;
            if (flag)
            {
                yield return new Gizmo_ShieldStressStatus
                {
                    shieldGen = this
                };
            }
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            yield break;
        }

        public bool ThreatDisabled(IAttackTargetSearcher disabledFor)
        {
            bool flag = this.CurShieldStress >= 1f;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = !disabledFor.CurrentEffectiveVerb.IsEMP();
                result = (flag2 || !this.CanFunction);
            }
            return result;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<int>(ref ticksToReset, "ticksToReset");
            Scribe_Values.Look<float>(ref curShieldStress, "shieldStress");
            Scribe_Values.Look<int>(ref shieldBuffer, "shieldBuffer");
            Scribe_Values.Look<bool>(ref active, "active", false, false);
            Scribe_Values.Look<int>(ref lastAbsorbDamageTick, "lastAbsorbDamageTick");
            Scribe_Collections.Look<Thing, int>(ref this.affectedThings, "affectedThings", LookMode.Reference, LookMode.Value, ref this.affectedThingsKeysWorkingList, ref this.affectedThingsValuesWorkingList);
            base.ExposeData();
        }
    }
}
