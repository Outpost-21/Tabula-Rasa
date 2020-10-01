using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public class TraitEntry
	{
		public string defName;

		public int degree;

		public float chance = 100f;

		public float commonalityMale = -1f;

		public float commonalityFemale = -1f;
	}
}
