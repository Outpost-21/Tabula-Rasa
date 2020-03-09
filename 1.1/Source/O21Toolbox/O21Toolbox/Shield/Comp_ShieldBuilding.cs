using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Shield
{
    [StaticConstructorOnStartup]
    public class Comp_ShieldBuilding : ThingComp
	{
        public CompProperties_ShieldBuilding Props => (CompProperties_ShieldBuilding)props;

		private int lastInterceptTicks = -999999;
		private int lastHitByEmpTicks = -999999;
		private float lastInterceptAngle;
		private bool debugInterceptNonHostileProjectiles = true;
		private static readonly Material ForceFieldMat = MaterialPool.MatFrom("Other/ForceField", ShaderDatabase.MoteGlow);
		private static readonly Material ForceFieldConeMat = MaterialPool.MatFrom("Other/ForceFieldCone", ShaderDatabase.MoteGlow);
		private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();
		private const float TextureActualRingSizeFactor = 1.1601562f;
		private static readonly Color InactiveColor = new Color(0.2f, 0.2f, 0.2f);

		private bool showShieldToggle = false;
		public float CurStressLevel = 0f;
		public float MaxStressLevel = 1f;

		public int ticksToReset;
		public bool overloaded;
		public bool activeLastTick;

		private bool checkedPowerComp = false;
		private CompPowerTrader cachedPowerComp;

		public List<Building> connectedVents = new List<Building>();

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref lastInterceptTicks, "lastInterceptTicks", -999999);
			Scribe_Values.Look<int>(ref lastHitByEmpTicks, "lastHitByEmpTicks", -999999);
			Scribe_Values.Look<bool>(ref showShieldToggle, "showShieldToggle", false);
			Scribe_Values.Look<float>(ref CurStressLevel, "curStressLevel", 0f);
			Scribe_Values.Look<float>(ref MaxStressLevel, "maxStressLevel", 1f);
			Scribe_Values.Look<int>(ref ticksToReset, "ticksToReset", -1);
			Scribe_Values.Look<bool>(ref overloaded, "overloaded", false);
			Scribe_Values.Look<bool>(ref activeLastTick, "activeLastTick", false);
		}

		public bool Active => !overloaded && (powerTrader == null || powerTrader.PowerOn);

		public bool OnCooldown => Find.TickManager.TicksGame < this.lastInterceptTicks + this.Props.cooldownTicks;

		public bool Disarmed => Find.TickManager.TicksGame < this.lastHitByEmpTicks + this.Props.disarmedByEmpForTicks;

		public int CooldownTicksLeft
		{
			get
			{
				if (!this.OnCooldown)
				{
					return 0;
				}
				return this.Props.cooldownTicks - (Find.TickManager.TicksGame - this.lastInterceptTicks);
			}
		}

		public int DisarmedTicksLeft
		{
			get
			{
				if (!this.Disarmed)
				{
					return 0;
				}
				return this.Props.disarmedByEmpForTicks - (Find.TickManager.TicksGame - this.lastHitByEmpTicks);
			}
		}
		public bool ReactivatedThisTick => Find.TickManager.TicksGame - this.lastInterceptTicks == this.Props.cooldownTicks;

		public bool CheckIntercept(Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
		{
			Vector3 vector = this.parent.Position.ToVector3Shifted();
			float num = this.Props.radius + projectile.def.projectile.SpeedTilesPerTick + 0.1f;
			if ((newExactPos.x - vector.x) * (newExactPos.x - vector.x) + (newExactPos.z - vector.z) * (newExactPos.z - vector.z) > num * num)
			{
				return false;
			}
			if (!this.Active)
			{
				return false;
			}
			bool flag;
			if (this.Props.interceptGroundProjectiles)
			{
				flag = !projectile.def.projectile.flyOverhead;
			}
			else
			{
				flag = (this.Props.interceptAirProjectiles && projectile.def.projectile.flyOverhead);
			}
			if (!flag)
			{
				return false;
			}
			if ((projectile.Launcher == null || !projectile.Launcher.HostileTo(this.parent)) && !this.debugInterceptNonHostileProjectiles)
			{
				return false;
			}
			if ((new Vector2(vector.x, vector.z) - new Vector2(lastExactPos.x, lastExactPos.z)).sqrMagnitude <= this.Props.radius * this.Props.radius)
			{
				return false;
			}
			if (!GenGeo.IntersectLineCircleOutline(new Vector2(vector.x, vector.z), this.Props.radius, new Vector2(lastExactPos.x, lastExactPos.z), new Vector2(newExactPos.x, newExactPos.z)))
			{
				return false;
			}
			this.lastInterceptAngle = lastExactPos.AngleToFlat(this.parent.TrueCenter());
			this.lastInterceptTicks = Find.TickManager.TicksGame;
			if (projectile.def.projectile.damageDef == DamageDefOf.EMP)
			{
				this.lastHitByEmpTicks = Find.TickManager.TicksGame;
			}
			Effecter effecter = new Effecter(EffecterDefOf.Interceptor_BlockedProjectile);
			effecter.Trigger(new TargetInfo(newExactPos.ToIntVec3(), this.parent.Map, false), TargetInfo.Invalid);
			effecter.Cleanup();
			UpdateStress(projectile);
			return true;
		}

		private CompPowerTrader powerTrader
		{
			get
			{
				if (!checkedPowerComp)
				{
					cachedPowerComp = parent.GetComp<CompPowerTrader>();
					checkedPowerComp = true;
				}
				return cachedPowerComp;
			}
		}

		public void UpdateStress()
		{
			float tempChange = 0f;

			if (CurStressLevel <= 0)
			{
				tempChange += Props.heatGenFactorPassive * Props.heatGenBase;
			}
			else
			{
				tempChange += Props.heatGenFactorActive * Props.heatGenBase;
			}
			if (parent.AmbientTemperature > Props.maximumHeatLevel)
			{
				tempChange += parent.AmbientTemperature - Props.maximumHeatLevel;
			}
			if (!connectedVents.NullOrEmpty())
			{
				foreach(Building vent in connectedVents)
				{
					tempChange += vent.AmbientTemperature;
				}
			}

			CurStressLevel = Mathf.Clamp(CurStressLevel + (tempChange * 0.01f / 60) , 0f, MaxStressLevel);
			if(CurStressLevel >= MaxStressLevel)
			{
				OverloadShield();
			}
		}

		public void UpdateStress(Projectile projectile)
		{
			CurStressLevel += projectile.DamageAmount * Props.stressPerDamage;
			UpdateStress();
		}

		public void OverloadShield()
		{
			Props.breakSound.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
			MoteMaker.MakeStaticMote(parent.TrueCenter(), parent.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = parent.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
				MoteMaker.ThrowDustPuff(loc, parent.Map, Rand.Range(0.8f, 1.2f));
			}
			ticksToReset = Props.resetTime;
			overloaded = true;
			CurStressLevel = 0f;
		}

		public void UpdatePowerUsage()
		{
			if(CurStressLevel <= 0)
			{
				powerTrader.Props.basePowerConsumption = Props.powerUsageBase * (Props.powerUsageFactorPassive * Props.powerUsageFactorScale.max);
			}
			else
			{
				powerTrader.Props.basePowerConsumption = Props.powerUsageBase * ((CurStressLevel * Props.powerUsageFactorActive) * Props.powerUsageFactorScale.max);
			}
		}

		public override void CompTick()
		{
			if (this.ReactivatedThisTick && this.Props.reactivateEffect != null)
			{
				Effecter effecter = new Effecter(this.Props.reactivateEffect);
				effecter.Trigger(this.parent, TargetInfo.Invalid);
				effecter.Cleanup();
			}
			if (overloaded)
			{
				ticksToReset--;
				if(ticksToReset <= 0)
				{
					overloaded = false;
				}
			}
			else
			{
				UpdateStress();
			}
			
			if(powerTrader != null)
			{
				UpdatePowerUsage();
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			Vector3 pos = this.parent.Position.ToVector3Shifted();
			pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
			float currentAlpha = this.GetCurrentAlpha();
			if (currentAlpha > 0f)
			{
				Color value;
				if (this.Active || !Find.Selector.IsSelected(this.parent))
				{
					value = this.Props.color;
				}
				else
				{
					value = InactiveColor;
				}
				value.a *= currentAlpha;
				MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(this.Props.radius * 2f * 1.1601562f, 1f, this.Props.radius * 2f * 1.1601562f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, ForceFieldMat, 0, null, 0, MatPropertyBlock);
			}
			float currentConeAlpha_RecentlyIntercepted = this.GetCurrentConeAlpha_RecentlyIntercepted();
			if (currentConeAlpha_RecentlyIntercepted > 0f)
			{
				Color color = this.Props.color;
				color.a *= currentConeAlpha_RecentlyIntercepted;
				MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
				Matrix4x4 matrix2 = default(Matrix4x4);
				matrix2.SetTRS(pos, Quaternion.Euler(0f, this.lastInterceptAngle - 90f, 0f), new Vector3(this.Props.radius * 2f * 1.1601562f, 1f, this.Props.radius * 2f * 1.1601562f));
				Graphics.DrawMesh(MeshPool.plane10, matrix2, ForceFieldConeMat, 0, null, 0, MatPropertyBlock);
			}
		}

		private float GetCurrentAlpha()
		{
			return Mathf.Max(Mathf.Max(Mathf.Max(this.GetCurrentAlpha_Idle(), this.GetCurrentAlpha_Selected()), this.GetCurrentAlpha_RecentlyIntercepted()), this.GetCurrentAlpha_RecentlyActivated());
		}

		private float GetCurrentAlpha_Idle()
		{
			if (!this.Active)
			{
				return 0f;
			}
			if (this.parent.Faction == Faction.OfPlayer && !this.debugInterceptNonHostileProjectiles)
			{
				return 0f;
			}
			if (Find.Selector.IsSelected(this.parent))
			{
				return 0f;
			}
			return Mathf.Lerp(-1.7f, 0.11f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 96804938) % 100) + Time.realtimeSinceStartup * 0.7f) + 1f) / 2f);
		}

		private float GetCurrentAlpha_Selected()
		{
			if (!Find.Selector.IsSelected(this.parent) || !showShieldToggle)
			{
				return 0f;
			}
			if (!this.Active)
			{
				return 0.41f;
			}
			return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * 2f) + 1f) / 2f);
		}
		private float GetCurrentAlpha_RecentlyIntercepted()
		{
			int num = Find.TickManager.TicksGame - this.lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.09f;
		}

		private float GetCurrentAlpha_RecentlyActivated()
		{
			if (!this.Active)
			{
				return 0f;
			}
			int num = Find.TickManager.TicksGame - (this.lastInterceptTicks + this.Props.cooldownTicks);
			return Mathf.Clamp01(1f - (float)num / 50f) * 0.09f;
		}

		private float GetCurrentConeAlpha_RecentlyIntercepted()
		{
			int num = Find.TickManager.TicksGame - this.lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.82f;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return new Gizmo_ShieldStatus
			{
				shield = this
			};
			yield return new Command_Toggle
			{
				defaultLabel = "Toggle Visibility",
				isActive = (() => this.showShieldToggle),
				toggleAction = delegate ()
				{
					this.showShieldToggle = !this.showShieldToggle;
				}
			};
			if (Prefs.DevMode)
			{
				if (this.OnCooldown)
				{
					yield return new Command_Action
					{
						defaultLabel = "Dev: Reset cooldown",
						action = delegate ()
						{
							this.lastInterceptTicks = Find.TickManager.TicksGame - this.Props.cooldownTicks;
						}
					};
				}
				yield return new Command_Toggle
				{
					defaultLabel = "Dev: Intercept non-hostile",
					isActive = (() => this.debugInterceptNonHostileProjectiles),
					toggleAction = delegate ()
					{
						this.debugInterceptNonHostileProjectiles = !this.debugInterceptNonHostileProjectiles;
					}
				};
			}
			yield break;
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.Props.interceptGroundProjectiles || this.Props.interceptAirProjectiles)
			{
				string value;
				if (this.Props.interceptGroundProjectiles)
				{
					value = "InterceptsProjectiles_GroundProjectiles".Translate();
					if (this.Props.interceptAirProjectiles)
					{
						value += "\n\n" + "InterceptsProjectiles_AerialProjectiles".Translate();
					}
				}
				else
				{
					value = "InterceptsProjectiles_AerialProjectiles".Translate();
				}
				if (this.Props.cooldownTicks > 0)
				{
					stringBuilder.Append("InterceptsProjectilesEvery".Translate(value, this.Props.cooldownTicks.ToStringTicksToPeriod(true, false, true, true)));
				}
				else
				{
					stringBuilder.Append("InterceptsProjectiles".Translate(value));
				}
			}
			if (this.OnCooldown)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("CooldownTime".Translate() + ": " + this.CooldownTicksLeft.ToStringTicksToPeriod(true, false, true, true));
			}
			if (this.Disarmed)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("DisarmedTime".Translate() + ": " + this.DisarmedTicksLeft.ToStringTicksToPeriod(true, false, true, true));
			}
			return stringBuilder.ToString();
		}

		public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.PostPostApplyDamage(dinfo, totalDamageDealt);
			if (dinfo.Def == DamageDefOf.EMP)
			{
				this.lastHitByEmpTicks = Find.TickManager.TicksGame;
			}
		}
	}
}
