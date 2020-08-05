using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
	public class Comp_PawnDeterrant : ThingComp
	{
		public CompProperties_PawnDeterrant Props => (CompProperties_PawnDeterrant)props;

		private int radius;
		public int Radius
		{
			get
			{
				return radius;
			}
			set
			{
				if (value < Props.radiusLimits.min)
				{
					radius = Props.radiusLimits.min;
					return;
				}
				if (value > Props.radiusLimits.max)
				{
					radius = Props.radiusLimits.max;
					return;
				}
				radius = (int)value;
			}
		}
	}
}
