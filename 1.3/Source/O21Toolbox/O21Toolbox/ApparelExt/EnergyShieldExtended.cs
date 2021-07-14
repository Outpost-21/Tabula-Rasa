using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace O21Toolbox.ApparelExt
{
    public class EnergyShieldExtended : Apparel
	{
		private float energy;
		private int ticksToReset = -1;
		private int lastKeepDisplayTick = -9999;
		private Vector3 impactAngleVect;
		private int lastAbsorbDamageTick = -9999;
		private Material bubbleMat;

		public DefModExt_EnergyShieldProps modExt => def.GetModExtension<DefModExt_EnergyShieldProps>();

		public float EnergyMax =>  modExt.energyShieldEnergyMax;

		public float EnergyGainPerTick => modExt.energyShieldRechargeRate / 60f;

		public float Energy => energy;

		protected virtual Material BubbleMat
		{
			get
			{
				if (bubbleMat == null)
				{
					if (modExt.shieldTexPath.NullOrEmpty())
					{
						bubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, modExt.shieldColor);
					}
					else
					{
						bubbleMat = MaterialPool.MatFrom(modExt.shieldTexPath, ShaderDatabase.Transparent, modExt.shieldColor);
					}
				}
				return bubbleMat;
			}
		}

		public ShieldState ShieldState
		{
			get
			{
				bool flag = ticksToReset > 0;
				ShieldState result;
				if (flag)
				{
					result = ShieldState.Resetting;
				}
				else
				{
					result = ShieldState.Active;
				}
				return result;
			}
		}

		private bool ShouldDisplay
		{
			get
			{
				Pawn wearer = base.Wearer;
				return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < lastKeepDisplayTick + 1000);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref energy, "energy", 0f, false);
			Scribe_Values.Look<int>(ref ticksToReset, "ticksToReset", -1, false);
			Scribe_Values.Look<int>(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
		}

		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			foreach (Gizmo gizmo in base.GetWornGizmos())
			{
				yield return gizmo;
			}
			if (Find.Selector.SingleSelectedThing == base.Wearer)
			{
				yield return new Gizmo_ShieldStatus
				{
					shield = this
				};
			}
			yield break;
		}

		public override float GetSpecialApparelScoreOffset()
		{
			return EnergyMax * 0.25f;
		}

		public override void Tick()
		{
			base.Tick();
			bool flag = base.Wearer == null;
			if (flag)
			{
				energy = 0f;
			}
			else
			{
				bool flag2 = ShieldState == ShieldState.Resetting;
				if (flag2)
				{
					ticksToReset--;
					bool flag3 = ticksToReset <= 0;
					if (flag3)
					{
						Reset();
					}
				}
				else
				{
					bool flag4 = ShieldState == ShieldState.Active;
					if (flag4)
					{
						energy += EnergyGainPerTick;
						bool flag5 = energy > EnergyMax;
						if (flag5)
						{
							energy = EnergyMax;
						}
					}
				}
			}
		}

		public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
		{
			bool result;
			if (ShieldState > ShieldState.Active)
			{
				result = false;
			}
			else
			{
				if ((dinfo.Def == DamageDefOf.EMP && modExt.shieldEMPBlock == ShieldEMPBlock.False || (modExt.shieldEMPBlock == ShieldEMPBlock.Pawn && Wearer.stances.stunner.AffectedByEMP)))
				{
					energy = 0f;
					Break();
					result = false;
				}
				else
				{
					if ((dinfo.Def.isRanged && modExt.blockRangedAttack)
						|| dinfo.Def.isExplosive
						|| (dinfo.Weapon == null && dinfo.Instigator is Pawn && modExt.blockMeleeAttack))
					{
						energy -= dinfo.Amount * 0.033f;
						if (energy < 0f)
						{
							Break();
						}
						else
						{
							AbsorbedDamage(dinfo);
						}
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public void KeepDisplaying()
		{
			lastKeepDisplayTick = Find.TickManager.TicksGame;
		}

		private void AbsorbedDamage(DamageInfo dinfo)
		{
			SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
			Vector3 loc = base.Wearer.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
			float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
			FleckMaker.MakeStaticMote(loc, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
			int num2 = (int)num;
			for (int i = 0; i < num2; i++)
			{
				FleckMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			lastAbsorbDamageTick = Find.TickManager.TicksGame;
			KeepDisplaying();
		}

		private void Break()
		{
			SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			FleckMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				FleckMaker.ThrowDustPuff(base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			energy = 0f;
			ticksToReset = 3200;
		}

		private void Reset()
		{
			bool spawned = base.Wearer.Spawned;
			if (spawned)
			{
				SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
				FleckMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
			}
			ticksToReset = -1;
			energy = 0.2f;
		}

		public override void DrawWornExtras()
		{
			bool flag = ShieldState == ShieldState.Active && ShouldDisplay;
			if (flag)
			{
				float num = Mathf.Lerp(modExt.minShieldSize, modExt.maxShieldSize, energy);
				Vector3 vector = base.Wearer.Drawer.DrawPos;
				vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
				bool flag2 = num2 < 8;
				if (flag2)
				{
					float num3 = (float)((8 - num2) / 8) * 0.05f;
					vector += impactAngleVect * num3;
					num -= num3;
				}
				float angle = (float)Rand.Range(0, 360);
				Vector3 s = new Vector3(num, 1f, num);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
			}
		}

		public override bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
		{
			return true;
		}
	}
}
