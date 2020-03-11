using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

using O21Toolbox.Utility;

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

		public int shieldOffsetX = 0;
		public int shieldOffsetY = 0;

		public int curShieldRadius = -1;

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
			Scribe_Values.Look<int>(ref shieldOffsetX, "shieldOffsetX", 0);
			Scribe_Values.Look<int>(ref shieldOffsetY, "shieldOffsetX", 0);
			Scribe_Values.Look<int>(ref curShieldRadius, "curShieldRadius", Props.shieldScaleDefault);
		}

		public bool Active => !overloaded && (powerTrader == null || powerTrader.PowerOn);

		public bool OnCooldown => Find.TickManager.TicksGame < this.lastInterceptTicks + this.Props.cooldownTicks;

		public bool Disarmed => Find.TickManager.TicksGame < this.lastHitByEmpTicks + this.Props.disarmedByEmpForTicks;

		public Vector3 CurShieldPosition => new IntVec3(parent.Position.x + shieldOffsetX, parent.Position.y, parent.Position.z + shieldOffsetY).ToVector3Shifted(); 
		
		public int SetShieldRadius
		{
			get => CurShieldRadius;
			set
			{
				if (value < Props.shieldScaleLimits.min)
				{
					curShieldRadius = Props.shieldScaleLimits.min;
					return;
				}
				if (value > Props.shieldScaleLimits.max)
				{
					curShieldRadius = Props.shieldScaleLimits.max;
					return;
				}
				curShieldRadius = (int)value;
			}
		}

		public int CurShieldRadius
		{
			get
			{
				return curShieldRadius;
			}
		}

		public int SetShieldOffsetX
		{
			get => shieldOffsetX;
			set
			{
				if (value < -CurShieldRadius)
				{
					shieldOffsetX = -CurShieldRadius;
					return;
				}
				if (value > CurShieldRadius)
				{
					shieldOffsetX = CurShieldRadius;
					return;
				}
				shieldOffsetX = (int)value;
			}
		}

		public int SetShieldOffsetY
		{
			get => shieldOffsetY;
			set
			{
				if (value < -CurShieldRadius)
				{
					shieldOffsetY = -CurShieldRadius;
					return;
				}
				if (value > CurShieldRadius)
				{
					shieldOffsetY = CurShieldRadius;
					return;
				}
				shieldOffsetY = (int)value;
			}
		}

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

		public bool HasPowerTrader => this.powerTrader != null;

		public bool ReactivatedThisTick => Find.TickManager.TicksGame - this.lastInterceptTicks == this.Props.cooldownTicks;

		public bool CheckIntercept(Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
		{
			//Vector3 vector = this.parent.Position.ToVector3Shifted();
			Vector3 vector = CurShieldPosition;
			float num = CurShieldRadius + projectile.def.projectile.SpeedTilesPerTick + 0.1f;
			if ((newExactPos.x - vector.x) * (newExactPos.x - vector.x) + (newExactPos.z - vector.z) * (newExactPos.z - vector.z) > num * num)
			{
				return false;
			}
			if (!this.Active)
			{
				return false;
			}
			bool flag = false;
			if (this.Props.interceptGroundProjectiles)
			{
				flag = !projectile.def.projectile.flyOverhead;
			}
			if(this.Props.interceptAirProjectiles)
			{
				flag = projectile.def.projectile.flyOverhead;
			}
			if(Props.interceptAirProjectiles && Props.interceptGroundProjectiles)
			{
				flag = true;
			}
			if (!flag)
			{
				return false;
			}
			if ((projectile.Launcher == null || !projectile.Launcher.HostileTo(this.parent)) && !this.debugInterceptNonHostileProjectiles)
			{
				return false;
			}
			if ((new Vector2(vector.x, vector.z) - new Vector2(lastExactPos.x, lastExactPos.z)).sqrMagnitude <= CurShieldRadius * CurShieldRadius)
			{
				return false;
			}
			if (!GenGeo.IntersectLineCircleOutline(new Vector2(vector.x, vector.z), CurShieldRadius, new Vector2(lastExactPos.x, lastExactPos.z), new Vector2(newExactPos.x, newExactPos.z)))
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

		public CompPowerTrader powerTrader
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

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (CurShieldRadius < Props.shieldScaleLimits.min)
			{
				SetShieldRadius = Props.shieldScaleDefault;
			}

			parent.Map.GetComponent<MapComp_ShieldList>().shieldGenList.Add(parent);
		}

		public void UpdateStress(bool tickUpdate = false)
		{
			if (tickUpdate)
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
					foreach (Building vent in connectedVents)
					{
						tempChange += vent.AmbientTemperature;
					}
				}

				CurStressLevel = Mathf.Clamp(CurStressLevel + (tempChange * 0.01f / 60), 0f, MaxStressLevel);
			}

			if(CurStressLevel >= MaxStressLevel)
			{
				OverloadShield();
			}
		}

		public void UpdateStress(Projectile projectile)
		{
			CurStressLevel = Mathf.Clamp(CurStressLevel + ((projectile.DamageAmount * Props.stressPerDamage) / 100f), 0f, MaxStressLevel);
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

			if (Props.explodeOnCollapse && parent.TryGetComp<CompExplosive>() != null)
			{
				parent.TryGetComp<CompExplosive>().StartWick();
			}
		}

		public void UpdatePowerUsage()
		{
			if(CurStressLevel <= 0)
			{
				powerTrader.PowerOutput = Props.powerUsageBase * (Props.powerUsageFactorPassive * Props.powerUsageFactorPassive);
			}
			else
			{
				powerTrader.PowerOutput = Props.powerUsageBase * (CurStressLevel * Props.powerUsageFactorActive);
			}
		}

		public override void CompTick()
		{
			if(powerTrader == null || powerTrader.PowerOn)
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
					if (ticksToReset <= 0)
					{
						overloaded = false;
					}
				}
				else
				{
					UpdateStress(true);
					if (CurStressLevel >= Props.shieldOverloadThreshold && Rand.Chance(Props.shieldOverloadChance * (1f - ((1f - CurStressLevel) * 10f))))
					{
						GenExplosion.DoExplosion(parent.OccupiedRect().ExpandedBy(Props.extraOverloadRange).RandomCell, parent.Map, 1.9f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
					}
				}
			}
			
			if(powerTrader != null)
			{
				UpdatePowerUsage();
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			//Vector3 pos = this.parent.Position.ToVector3Shifted();
			Vector3 pos = CurShieldPosition;
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
				matrix.SetTRS(pos, Quaternion.identity, new Vector3((float)CurShieldRadius * 2f * 1.1601562f, 1f, (float)CurShieldRadius * 2f * 1.1601562f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, ForceFieldMat, 0, null, 0, MatPropertyBlock);
			}
			float currentConeAlpha_RecentlyIntercepted = this.GetCurrentConeAlpha_RecentlyIntercepted();
			if (currentConeAlpha_RecentlyIntercepted > 0f)
			{
				Color color = this.Props.color;
				color.a *= currentConeAlpha_RecentlyIntercepted;
				MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
				Matrix4x4 matrix2 = default(Matrix4x4);
				matrix2.SetTRS(pos, Quaternion.Euler(0f, this.lastInterceptAngle - 90f, 0f), new Vector3((float)CurShieldRadius * 2f * 1.1601562f, 1f, (float)CurShieldRadius * 2f * 1.1601562f));
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
			if (showShieldToggle)
			{
				return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * 2f) + 1f) / 2f);
			}
			return Mathf.Lerp(-1.7f, 0.11f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 96804938) % 100) + Time.realtimeSinceStartup * 0.7f) + 1f) / 2f);
		}

		private float GetCurrentAlpha_Selected()
		{
			if (!Find.Selector.IsSelected(this.parent))
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
			if(parent.Faction == Faction.OfPlayer)
			{
				yield return new Gizmo_ShieldStatus
				{
					shield = this
				};
				if (Props.shieldCanBeScaled)
				{
					yield return new Command_Action
					{
						defaultLabel = "Set Radius",
						defaultDesc = "Set the shields current radius.",
						action = () => Find.WindowStack.Add(new Popup_IntSlider("Radius", Props.shieldScaleLimits.min, Props.shieldScaleLimits.max, () => (int)SetShieldRadius, size => SetShieldRadius = size))
					};
				}
				if (Props.shieldCanBeOffset)
				{
					yield return new Command_Action
					{
						defaultLabel = "Set Offset X",
						defaultDesc = "Set the shields east/west offset.",
						action = () => Find.WindowStack.Add(new Popup_IntSlider("X Offset", -CurShieldRadius + 1, CurShieldRadius - 1, () => (int)SetShieldOffsetX, size => SetShieldOffsetX = size))
					};
					yield return new Command_Action
					{
						defaultLabel = "Set Offset Y",
						defaultDesc = "Set the shields north/south offset.",
						action = () => Find.WindowStack.Add(new Popup_IntSlider("Y Offset", -CurShieldRadius + 1, CurShieldRadius - 1, () => (int)SetShieldOffsetY, size => SetShieldOffsetY = size))
					};
				}
				yield return new Command_Toggle
				{
					defaultLabel = "Toggle Visibility",
					isActive = (() => this.showShieldToggle),
					toggleAction = delegate ()
					{
						this.showShieldToggle = !this.showShieldToggle;
					}
				};
			}
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
						value += ("\n" + "InterceptsProjectiles_AerialProjectiles".Translate());
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
				stringBuilder.Append("CooldownTime".Translate() + ": " + this.ticksToReset.ToStringTicksToPeriod(true, false, true, true));
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

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			parent.Map.GetComponent<MapComp_ShieldList>().shieldGenList.Remove(parent);
			base.PostDestroy(mode, previousMap);
		}
	}
}
