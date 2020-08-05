using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class Biome_Replacement : DefModExtension
	{
		public static readonly Biome_Replacement Default = new Biome_Replacement();
		public bool replaceElevation = false;
		public int elevationMin = 10;
		public int elevationMax = 100;
		public Hilliness? replaceHilliness = null;
		public TerrainDef sandReplacement;
		public TerrainDef gravelReplacement;

		public Biome_Replacement()
        {
            this.ResolveReferences();
        }

		public override IEnumerable<string> ConfigErrors()
		{
			this.ResolveReferences();
			yield break;
		}

		private void ResolveReferences()
		{
			if (this.sandReplacement == null)
			{
				this.sandReplacement = TerrainDefOf.NormalSand;
			}
			if (this.gravelReplacement == null)
			{
				this.gravelReplacement = TerrainDefOf.NormalGravel;
			}
		}
	}
}
