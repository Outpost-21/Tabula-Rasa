using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
	public class PlaceWorker_ShowShieldRadius : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			CompProperties_Shield compProperties = def.GetCompProperties<CompProperties_Shield>();
			if (compProperties != null)
			{
				GenDraw.DrawCircleOutline(center.ToVector3Shifted(), compProperties.shieldScaleLimits.max, SimpleColor.Red);
				GenDraw.DrawCircleOutline(center.ToVector3Shifted(), compProperties.shieldScaleDefault, SimpleColor.White);
				GenDraw.DrawCircleOutline(center.ToVector3Shifted(), compProperties.shieldScaleLimits.min, SimpleColor.Green);
			}
		}

	}
}
