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
    public class Gene_Effecter : Gene
    {
		public Effecter effecter;

        public override void Tick()
        {
            base.Tick();
			if (pawn.Spawned)
			{
				if (effecter == null)
				{
					effecter = def.GetModExtension<DefModExt_GeneEffecter>().effecter.SpawnAttached(pawn, pawn.MapHeld);
				}
				effecter?.EffectTick(pawn, pawn);
			}
			else
			{
				effecter?.Cleanup();
				effecter = null;
			}
		}
    }
}
