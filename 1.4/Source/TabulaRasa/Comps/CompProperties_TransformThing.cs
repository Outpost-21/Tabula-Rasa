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
    public class CompProperties_TransformThing : CompProperties
	{
		public CompProperties_TransformThing()
        {
			compClass = typeof(Comp_TransformThing);
        }

		public ThingDef thingDef;

		public string texPath;

		public string label;

		public string desc;

		public bool onlyWhenHealthFull = true;

		public FleckDef fleck;
	}
}
