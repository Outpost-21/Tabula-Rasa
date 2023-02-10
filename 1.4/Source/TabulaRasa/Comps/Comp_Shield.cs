using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Grammar;
using Verse.Sound;

namespace TabulaRasa
{
	[StaticConstructorOnStartup]
	public class Comp_Shield : ThingComp
	{
		public CompProperties_Shield Props => (CompProperties_Shield)props;

		private int lastInterceptTicks = -999999;
		private int lastHitByEmpTicks = -999999;
		private float lastInterceptAngle;
		private bool debugInterceptNonHostileProjectiles = true;
		private static readonly Material ForceFieldMat = MaterialPool.MatFrom("Other/ForceField", ShaderDatabase.MoteGlow);
		private static readonly Material ForceFieldConeMat = MaterialPool.MatFrom("Other/ForceFieldCone", ShaderDatabase.MoteGlow);
		private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();
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

		private CompPowerTrader cachedPowerComp;
		private CompFlickable cachedFlickableComp;
		private CompHeatPusher cachedHeatComp;

		public float lastTempChange;

		public Color currentColor;

		public virtual bool Active => !overloaded && (PowerTrader == null || PowerTrader.PowerOn) && (Flicker == null || Flicker.SwitchIsOn);

		public Vector3 CurShieldPosition => new IntVec3(parent.Position.x + shieldOffsetX, parent.Position.y, parent.Position.z + shieldOffsetY).ToVector3Shifted();

		public int SetShieldRadius { get => curShieldRadius; set => curShieldRadius = Mathf.Clamp(value, Props.shieldScaleLimits.min, Props.shieldScaleLimits.max); }

		public int SetShieldOffsetX { get => shieldOffsetX; set => shieldOffsetX = value; }

		public int SetShieldOffsetY { get => shieldOffsetY; set => shieldOffsetY = value; }

		public bool HasPowerTrader => PowerTrader != null;

		public bool ReactivatedThisTick => Find.TickManager.TicksGame - lastInterceptTicks == Props.resetTime;

		public float ScaleDamageFactor => Mathf.Lerp(0.5f, 2.0f, GetShieldScalePercentage);

		public float GetShieldScalePercentage => Props.shieldCanBeScaled ? ((curShieldRadius - Props.shieldScaleLimits.min) / (Props.shieldScaleLimits.max - Props.shieldScaleLimits.min)) : 1f;

		public CompPowerTrader PowerTrader { get { if (cachedPowerComp == null) { cachedPowerComp = parent.GetComp<CompPowerTrader>(); } return cachedPowerComp; } }

		public CompFlickable Flicker { get { if (cachedFlickableComp == null) { cachedFlickableComp = parent.GetComp<CompFlickable>(); } return cachedFlickableComp; } }

		public CompHeatPusher HeatComp { get { if (cachedHeatComp == null) { cachedHeatComp = parent.GetComp<CompHeatPusher>(); } return cachedHeatComp; } }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!ModLister.CheckRoyalty("Projectile interception"))
			{
				LogUtil.LogMessage("Shield Setup skipped because user lacks Royalty.");
				parent.Destroy();
				return;
			}

			base.PostSpawnSetup(respawningAfterLoad);

			if (curShieldRadius < Props.shieldScaleLimits.min)
			{
				SetShieldRadius = Props.shieldScaleDefault;
			}
			if (!respawningAfterLoad)
			{
				currentColor = Props.shieldColour;
				SetShieldRadius = Props.shieldScaleDefault;
			}

			parent.Map.GetComponent<MapComp_ShieldList>().shieldGenList.Add(parent);
		}

		public bool CheckIntercept(Skyfaller skyfaller)
		{
			if (!skyfaller?.GetDirectlyHeldThings()?.Any(ht => ht.Faction == Faction.OfPlayer) ?? true)
			{
				if (Active && Props.podBlocker)
				{
					if (skyfaller.Position.DistanceTo(CurShieldPosition.ToIntVec3()) <= curShieldRadius) { return true; }
				}
			}
			return false;
		}

		public bool BombardmentCanStartFireAt(Bombardment bombardment, IntVec3 cell)
		{
			return !this.Active || !this.Props.interceptAirProjectiles || ((bombardment.instigator == null || !bombardment.instigator.HostileTo(this.parent)) && !debugInterceptNonHostileProjectiles && !Props.interceptNonHostileProjectiles) || !cell.InHorDistOf(this.parent.Position, this.curShieldRadius);
		}

		public bool CheckIntercept(Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
		{
			if (!ModLister.CheckRoyalty("Projectile interception"))
			{
				return false;
			}
			Vector3 vector = CurShieldPosition;
			float num = curShieldRadius + projectile.def.projectile.SpeedTilesPerTick + 0.1f;
			if ((newExactPos.x - vector.x) * (newExactPos.x - vector.x) + (newExactPos.z - vector.z) * (newExactPos.z - vector.z) > num * num)
			{
				return false;
			}
			if (!this.Active)
			{
				return false;
			}
			if (!Comp_Shield.InterceptsProjectile(Props, projectile))
			{
				return false;
			}
			if ((projectile.Launcher == null || !projectile.Launcher.HostileTo(parent)) && !debugInterceptNonHostileProjectiles && Props.interceptNonHostileProjectiles)
			{
				return false;
			}
			if (!Props.interceptOutgoingProjectiles && (new Vector2(vector.x, vector.z) - new Vector2(lastExactPos.x, lastExactPos.z)).sqrMagnitude <= curShieldRadius * curShieldRadius)
			{
				return false;
			}
			if (!GenGeo.IntersectLineCircleOutline(new Vector2(vector.x, vector.z), curShieldRadius, new Vector2(lastExactPos.x, lastExactPos.z), new Vector2(newExactPos.x, newExactPos.z)))
			{
				return false;
			}
			lastInterceptAngle = lastExactPos.AngleToFlat(parent.TrueCenter());
			lastInterceptTicks = Find.TickManager.TicksGame;
			if (projectile.def.projectile.damageDef == DamageDefOf.EMP)
			{
				lastHitByEmpTicks = Find.TickManager.TicksGame;
			}
			TriggerEffecter(newExactPos.ToIntVec3());
			UpdateStress(projectile);
			return true;
		}

		public static bool InterceptsProjectile(CompProperties_Shield props, Projectile projectile)
		{
			if (props.interceptGroundProjectiles && !projectile.def.projectile.flyOverhead) { return true; }
			if (props.interceptAirProjectiles && projectile.def.projectile.flyOverhead) { return true; }
			return false;
		}

		public void TriggerEffecter(IntVec3 pos)
		{
			Effecter effecter = new Effecter(EffecterDefOf.Interceptor_BlockedProjectile);
			effecter.Trigger(new TargetInfo(pos, this.parent.Map, false), TargetInfo.Invalid);
			effecter.Cleanup();
		}

		public void UpdateStress(bool tickUpdate = false, bool cooling = false)
		{
			if (tickUpdate)
			{
				float tempChange = 0f;

				if (Props.useAmbientCooling)
				{
					if (parent.AmbientTemperature > Props.maximumHeatLevel && !cooling)
					{
						tempChange += Props.stressReduction;
					}
					else
					{
						tempChange -= Props.stressReduction;
					}
				}
				else
				{
					tempChange -= Props.stressReduction;
				}
				if (!Active)
				{
					tempChange = -Props.stressReduction;
				}
				lastTempChange = (tempChange * 0.01f / 60);
				CurStressLevel = Mathf.Clamp(CurStressLevel + lastTempChange, 0f, MaxStressLevel);
			}

			if (CurStressLevel >= MaxStressLevel)
			{
				OverloadShield();
			}
		}

		public void UpdateStress(Projectile projectile)
		{
			float damage = ((projectile.DamageAmount * Props.stressPerDamage) / 100f);
			if (projectile.def.projectile.damageDef == DamageDefOf.EMP)
			{
				damage = damage * Props.empDamageFactor;
			}
			CurStressLevel = Mathf.Clamp(CurStressLevel + (damage * ScaleDamageFactor), 0f, MaxStressLevel);
			UpdateStress();
		}

		public void UpdateStress(Skyfaller skyfaller)
		{
			float damage = ((30000f * Props.stressPerDamage) / 100f);
			DropPodIncoming dropPodIncoming = skyfaller as DropPodIncoming;
			if (dropPodIncoming != null)
			{
				damage = damage / 3f;
			}
			CurStressLevel = Mathf.Clamp(CurStressLevel + (damage * ScaleDamageFactor), 0f, MaxStressLevel);
			UpdateStress();
		}

		public void OverloadShield()
		{
			if (Props.breakSound != null)
			{
				Props.breakSound.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
			}
			FleckMaker.ThrowExplosionInterior(parent.TrueCenter(), parent.Map, FleckDefOf.ExplosionFlash);
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = parent.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
				FleckMaker.ThrowDustPuff(loc, parent.Map, Rand.Range(0.8f, 1.2f));
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
			if (Active)
			{
				PowerTrader.PowerOutput = Props.powerUsageRange.LerpThroughRange(GetShieldScalePercentage);
			}
			else
			{
				PowerTrader.PowerOutput = 0f;
			}

		}

		public override void CompTick()
		{
			if (PowerTrader == null || PowerTrader.PowerOn)
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
						GenExplosion.DoExplosion(parent.OccupiedRect().ExpandedBy(Props.extraOverloadRange).RandomCell, parent.Map, 1.9f, DamageDefOf.EMP, null, -1, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null);
					}
				}
			}

			UpdateStress(true, false);

			if (PowerTrader != null)
			{
				UpdatePowerUsage();
			}

			if (HeatComp != null)
			{
				UpdateHeatPusher();
			}
		}

		public void UpdateHeatPusher()
		{
			if (Active)
			{
				HeatComp.Props.heatPerSecond = Props.heatGenRange.LerpThroughRange(CurStressLevel);
			}
			else
			{
				HeatComp.Props.heatPerSecond = 0f;
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			Vector3 pos = CurShieldPosition;
			pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
			float currentAlpha = this.GetCurrentAlpha();
			if (currentAlpha > 0f)
			{
				Color value;
				if (this.Active || !Find.Selector.IsSelected(this.parent))
				{
					value = currentColor;
				}
				else
				{
					value = InactiveColor;
				}
				value.a *= currentAlpha;
				Comp_Shield.MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(pos, Quaternion.identity, new Vector3((float)curShieldRadius * 2f * 1.1601562f, 1f, (float)curShieldRadius * 2f * 1.1601562f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, Comp_Shield.ForceFieldMat, 0, null, 0, Comp_Shield.MatPropertyBlock);
			}
			float currentConeAlpha_RecentlyIntercepted = this.GetCurrentConeAlpha_RecentlyIntercepted();
			if (currentConeAlpha_RecentlyIntercepted > 0f)
			{
				Color color = this.currentColor;
				color.a *= currentConeAlpha_RecentlyIntercepted;
				Comp_Shield.MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
				Matrix4x4 matrix2 = default(Matrix4x4);
				matrix2.SetTRS(pos, Quaternion.Euler(0f, this.lastInterceptAngle - 90f, 0f), new Vector3((float)curShieldRadius * 2f * 1.1601562f, 1f, (float)curShieldRadius * 2f * 1.1601562f));
				Graphics.DrawMesh(MeshPool.plane10, matrix2, Comp_Shield.ForceFieldConeMat, 0, null, 0, Comp_Shield.MatPropertyBlock);
			}
		}

		private float GetCurrentAlpha()
		{
			return Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(this.GetCurrentAlpha_Idle(), this.GetCurrentAlpha_Selected()), this.GetCurrentAlpha_RecentlyIntercepted()), this.GetCurrentAlpha_RecentlyActivated()), Props.minAlpha);
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
				float num = Mathf.Max(2f, Props.idlePulseSpeed);
				return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * num) + 1f) / 2f);
			}
			return Mathf.Lerp(-1.7f, 0.11f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 96804938) % 100) + Time.realtimeSinceStartup * 0.7f) + 1f) / 2f);
		}

		private float GetCurrentAlpha_Selected()
		{
			float num = Mathf.Max(2f, Props.idlePulseSpeed);
			if (!Find.Selector.IsSelected(this.parent))
			{
				return 0f;
			}
			if (!this.Active)
			{
				return 0.41f;
			}
			return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(this.parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * num) + 1f) / 2f);
		}

		public float GetCurrentAlpha_RecentlyIntercepted()
		{
			int num = Find.TickManager.TicksGame - this.lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.09f;
		}

		public float GetCurrentAlpha_RecentlyActivated()
		{
			if (!this.Active)
			{
				return 0f;
			}
			int num = Find.TickManager.TicksGame - (this.lastInterceptTicks + this.Props.resetTime);
			return Mathf.Clamp01(1f - (float)num / 50f) * 0.09f;
		}
		public float GetCurrentConeAlpha_RecentlyIntercepted()
		{
			if (!Props.drawInterceptCone)
			{
				return 0f;
			}
			int num = Find.TickManager.TicksGame - this.lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.82f;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo g in base.CompGetGizmosExtra())
			{
				yield return g;
			}
			if (parent.Faction == Faction.OfPlayer)
			{
				yield return new Gizmo_ShieldStatus
				{
					shield = this
				};
				if (Props.shieldCanBeColored)
				{
					yield return new Command_Action
					{
						defaultLabel = "ShieldGenColorLabel".Translate(),
						defaultDesc = "ShieldGenColorDescription".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Buttons/ShieldColor"),
						action = () => Find.WindowStack.Add(new Popup_ColourPicker(this))
					};
				}
				if (Props.shieldCanBeScaled)
				{
					yield return new Command_Action
					{
						defaultLabel = "ShieldGenRadiusLabel".Translate(),
						defaultDesc = "ShieldGenRadiusDescription".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Buttons/ShieldRadius"),
						action = () => Find.WindowStack.Add(new Popup_IntSlider("ShieldGenRadiusTitle".Translate(), Props.shieldScaleLimits.min, Props.shieldScaleLimits.max, () => (int)SetShieldRadius, size => SetShieldRadius = size))
					};
				}
				if (Props.shieldCanBeOffset)
				{
					yield return new Command_Action
					{
						defaultLabel = "ShieldGenOffsetXLabel".Translate(),
						defaultDesc = "ShieldGenOffsetXDescription".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Buttons/ShieldOffsetX"),
						action = () => Find.WindowStack.Add(new Popup_IntSlider("ShieldGenOffsetXTitle".Translate(), -(Props.shieldScaleLimits.max / 2), (Props.shieldScaleLimits.max / 2), () => (int)SetShieldOffsetX, size => SetShieldOffsetX = size))
					};
					yield return new Command_Action
					{
						defaultLabel = "ShieldGenOffsetYLabel".Translate(),
						defaultDesc = "ShieldGenOffsetYDescription".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Buttons/ShieldOffsetY"),
						action = () => Find.WindowStack.Add(new Popup_IntSlider("ShieldGenOffsetYTitle".Translate(), -(Props.shieldScaleLimits.max / 2), (Props.shieldScaleLimits.max / 2), () => (int)SetShieldOffsetY, size => SetShieldOffsetY = size))
					};
				}
				yield return new Command_Toggle
				{
					defaultLabel = "ShieldGenToggleVisibility".Translate(),
					defaultDesc = "ShieldGenToggleVisibilityDesc".Translate(),
					isActive = (() => this.showShieldToggle),
					icon = ContentFinder<Texture2D>.Get("UI/Buttons/ShieldVisibility"),
					toggleAction = delegate ()
					{
						this.showShieldToggle = !this.showShieldToggle;
					}
				};
			}
			if (Prefs.DevMode)
			{
				if (ticksToReset > 0)
				{
					yield return new Command_Action
					{
						defaultLabel = "Dev: Reset cooldown",
						action = delegate ()
						{
							ticksToReset = 0;
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
			if (Active)
			{
				if (ticksToReset > 0)
				{
					stringBuilder.Append("CooldownTime".Translate() + ": " + this.ticksToReset.ToStringTicksToPeriod(true, false, true, true));
				}
				else
				{
					stringBuilder.Append("Shield Active");
				}
			}
			else
			{
				stringBuilder.Append("Shield Inactive");
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

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref lastInterceptTicks, "lastInterceptTicks", -999999);
			Scribe_Values.Look(ref lastHitByEmpTicks, "lastHitByEmpTicks", -999999);
			Scribe_Values.Look(ref showShieldToggle, "showShieldToggle", false);
			Scribe_Values.Look(ref CurStressLevel, "curStressLevel", 0f);
			Scribe_Values.Look(ref MaxStressLevel, "maxStressLevel", 1f);
			Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1);
			Scribe_Values.Look(ref overloaded, "overloaded", false);
			Scribe_Values.Look(ref activeLastTick, "activeLastTick", false);
			Scribe_Values.Look(ref shieldOffsetX, "shieldOffsetX", 0);
			Scribe_Values.Look(ref shieldOffsetY, "shieldOffsetY", 0);
			Scribe_Values.Look(ref curShieldRadius, "curShieldRadius", Props.shieldScaleDefault);
			Scribe_Values.Look(ref currentColor, "currentColor", Props.shieldColour);
		}
	}
}
