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
	public class Building_GraveAdv : Building_Grave
	{
		public DefModExt_GraveAdv modExt => def.GetModExtension<DefModExt_GraveAdv>();

		public int CorpseCount => innerContainer.Count;

		public bool CanAcceptCorpses => CorpseCount < modExt.capacity;

		public int MaxAssignedPawnsCount => Math.Max(1, modExt.capacity - CorpseCount);

		public new bool StorageTabVisible => CanAcceptCorpses;

		public int nextDissolveTick = -1;

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref nextDissolveTick, "nextDissolveTick");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
			if (nextDissolveTick < 0)
			{
				ResetDissolveTimer();
			}
		}

        public override void TickRare()
        {
            base.TickRare();
			if(nextDissolveTick < 0)
            {
				ResetDissolveTimer();
            }
			if(modExt.dissolveCorpses && nextDissolveTick < Find.TickManager.TicksGame)
            {
				DissolveFirstCorpse();
				ResetDissolveTimer();
            }
        }

		public void DissolveFirstCorpse()
        {
			if(CorpseCount > 0)
			{
				innerContainer.First().Destroy();
			}
        }

		public void ResetDissolveTimer()
        {
			nextDissolveTick = modExt.dissolveTicks;
        }

        public override bool Accepts(Thing thing)
		{
			if (!innerContainer.CanAcceptAnyOf(thing))
			{
				return false;
			}
			if (!CanAcceptCorpses)
			{
				return false;
			}
			if (base.AssignedPawn != null)
			{
				if (!(thing is Corpse corpse))
				{
					return false;
				}
				if (corpse.InnerPawn != base.AssignedPawn)
				{
					return false;
				}
			}
			else if (!GetStoreSettings().AllowedToAccept(thing))
			{
				return false;
			}
			return true;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(InspectStringPartsFromComps());
			stringBuilder.Append($"\nCapacity: {CorpseCount.ToString()}/{modExt.capacity.ToString()}");
            if (modExt.dissolveCorpses && CorpseCount > 0)
            {
				stringBuilder.Append($"\nTime till next corpse dissolves: {(nextDissolveTick - Find.TickManager.TicksGame).TicksToDays()}");
            }
			return stringBuilder.ToString();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			IEnumerable<Gizmo> gizmos = base.GetGizmos();
			if (!base.StorageTabVisible && StorageTabVisible)
			{
				foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(GetStoreSettings()))
				{
					yield return item;
				}
			}
			foreach (Gizmo item2 in gizmos)
			{
				if ((item2 as Command_Action)?.defaultLabel != "CommandGraveAssignColonistLabel".Translate())
				{
					yield return item2;
				}
			}
		}
	}
}
