using System.Collections.Generic;
using UnityEngine;

namespace HRF
{
    public class ShieldProperties
	{
		public bool absorbMeleeDamage;
		public bool absorbRangeDamage;
		public int? maxAbsorb;
		public int? resourceConsumptionPerDamage;
		public float? damageAbsorbedPerResource;
		public int? postDamageDelay;
		public bool showGraphic = true;
		public string texPath = "Other/ShieldBubble";
		public Color shieldColor = Color.white;
		public VerbType cannotUseVerbType;
		public List<HediffResourceDef> activeWithHediffs;
	}
}
