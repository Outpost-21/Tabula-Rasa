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
	public class CompProperties_AdvFireOverlay : CompProperties_FireOverlay
	{
		public CompProperties_AdvFireOverlay()
		{
			compClass = typeof(Comp_AdvFireOverlay);
		}

		public List<Rot4> showRotations = new List<Rot4>();

		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			bool flag = this.showRotations.Count > 0;
			if (!flag)
			{
				this.showRotations.Add(Rot4.North);
				this.showRotations.Add(Rot4.South);
				this.showRotations.Add(Rot4.East);
				this.showRotations.Add(Rot4.West);
			}
		}
	}
}
