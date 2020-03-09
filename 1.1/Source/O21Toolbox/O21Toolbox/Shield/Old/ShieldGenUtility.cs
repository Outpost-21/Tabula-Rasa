using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Shield
{
    public static class ShieldGenUtility
	{
		public static bool BlockableByShield(this Projectile proj, Building_Shield shieldGen)
		{
			bool flag = !proj.def.projectile.flyOverhead;
			return flag || (!shieldGen.coveredCells.Contains(((Vector3)NonPublicFields.Projectile_origin.GetValue(proj)).ToIntVec3()) && (float)((int)NonPublicFields.Projectile_ticksToImpact.GetValue(proj)) / NonPublicProperties.Projectile_get_StartingTicksToImpact(proj) <= 0.5f);
		}
	}
}
