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
    public class IncidentWorker_CustomMeteoriteStrike : IncidentWorker
	{
		public override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 cell;
			DefModExt_CustomMeteoriteStrike modExt = def.GetModExtension<DefModExt_CustomMeteoriteStrike>();
			return TryFindCell(out cell, map, modExt.skyfaller);
		}

		public override bool TryExecuteWorker(IncidentParms parms)
		{
			DefModExt_CustomMeteoriteStrike modExt = def.GetModExtension<DefModExt_CustomMeteoriteStrike>();
			Map map = (Map)parms.target;
			if (!TryFindCell(out var cell, map, modExt.skyfaller))
			{
				return false;
			}
			List<Thing> list = modExt.thingSetMaker.root.Generate();
			SkyfallerMaker.SpawnSkyfaller(modExt.skyfaller, list, cell, map);
			LetterDef baseLetterDef = ((bool)list[0]?.def?.building?.isResourceRock ? LetterDefOf.PositiveEvent : LetterDefOf.NeutralEvent);
			string text = string.Format(def.letterText, list[0].def.label).CapitalizeFirst();
			SendStandardLetter(def.letterLabel + ": " + list[0].def.LabelCap, text, baseLetterDef, parms, new TargetInfo(cell, map));
			return true;
		}

		public bool TryFindCell(out IntVec3 cell, Map map, ThingDef skyfaller)
		{
			return CellFinderLoose.TryFindSkyfallerCell(skyfaller, map, out cell, alwaysAvoidColonists: true, allowCellsWithBuildings: false, allowCellsWithItems: false, allowRoofedCells: false);
		}
	}
}
