using System.Collections.Generic;
using System.Xml;
using Verse;

namespace HRF
{

    public class ResourceAdjustPerDamage
    {
		public float? flat;
		public float? point;
		public DamageDef damageDef;
        public HediffResourceDef hediff;
		public float GetResourceGain(float damageAmount)
        {
            float num = 0f;
			if (flat.HasValue)
            {
                num += flat.Value;
            }
            if (point.HasValue)
            {
                num += damageAmount * point.Value;
            }
            return num;
        }
	}
}
