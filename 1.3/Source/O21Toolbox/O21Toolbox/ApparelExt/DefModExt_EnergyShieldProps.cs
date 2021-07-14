using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class DefModExt_EnergyShieldProps : DefModExtension
    {
		public float energyShieldEnergyMax;
		public float energyShieldRechargeRate;
		public bool blockRangedAttack = true;
		public bool blockMeleeAttack;
		public string shieldTexPath;
		public bool showWhenDrafted;
		public bool showOnHostiles = true;
		public bool showOnNeutralInCombat;
		public float minShieldSize = 1.5f;
		public float maxShieldSize = 2f;
		public Color shieldColor = Color.white;
		public float energyLossPerDamage = 0.033f;
		public bool disableRotation;
		public int ticksToReset = 3200;
		public ShieldEMPBlock shieldEMPBlock = ShieldEMPBlock.False;
	}

	public enum ShieldEMPBlock
    {
		True,
		False,
		Pawn
    }
}
