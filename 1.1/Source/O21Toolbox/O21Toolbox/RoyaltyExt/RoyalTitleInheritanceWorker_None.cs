using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.RoyaltyExt
{
    public class RoyalTitleInheritanceWorker_None : RoyalTitleInheritanceWorker
	{
		public new Pawn FindHeir(Faction faction, Pawn pawn, RoyalTitleDef title)
		{
			return null;
		}
	}
}
