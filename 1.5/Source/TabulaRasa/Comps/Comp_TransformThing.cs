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
    public class Comp_TransformThing : ThingComp
	{
		public CompProperties_TransformThing Props => (CompProperties_TransformThing)props;

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo item in base.CompGetGizmosExtra())
			{
				yield return item;
			}
			if (Props.thingDef != null)
			{
				Building building = parent as Building;
				if (building.Faction != null && building.Faction.IsPlayer)
				{
					yield return new Command_Action
					{
						defaultLabel = Props.label,
						defaultDesc = Props.desc,
						icon = ContentFinder<Texture2D>.Get(Props.texPath),
						disabled = Disabled(building, Props.onlyWhenHealthFull),
						disabledReason = "TabulaRasa_CantTransformDamaged".Translate(),
						action = delegate
						{
							if (Props.fleck != null)
							{
								SpawnFleck(Props.fleck, building);
							}
							Transform(building, Props.thingDef);
						}
					};
				}
			}
		}

		public bool Disabled(Thing building, bool maxHealth)
		{
			return maxHealth && building.HitPoints < building.MaxHitPoints;
		}

		public void SpawnFleck(FleckDef fleck, Building building)
		{
			FleckCreationData dataStatic = FleckMaker.GetDataStatic(building.Position.ToVector3(), building.Map, fleck);
			dataStatic.rotationRate = 0.2f;
			building.Map.flecks.CreateFleck(dataStatic);
		}

		public void Transform(Building building, ThingDef thingDef)
		{
			IntVec3 position = building.Position;
			Map map = building.Map;
			building.Destroy();
			GenSpawn.Spawn(thingDef, position, map).SetFactionDirect(Faction.OfPlayer);
		}
	}
}
