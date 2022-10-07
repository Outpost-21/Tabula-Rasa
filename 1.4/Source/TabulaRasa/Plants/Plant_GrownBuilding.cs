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
    public class Plant_GrownBuilding : Plant
	{
		public override void Tick()
		{
			base.Tick();
			if (this.IsHashIntervalTick(2000))
			{
				TickLong();
			}
		}

		public override void TickLong()
		{
			base.TickLong();
			if (base.Destroyed)
			{
				return;
			}
			DefModExt_GrownBuilding modExt = def.GetModExtension<DefModExt_GrownBuilding>();
			if (modExt != null && modExt.matureInto != null && Growth >= 1f)
			{
				IntVec3 position = base.Position;
				Map map = base.Map;
				Thing thing = GenSpawn.Spawn(modExt.matureInto, position, map);
				thing.SetFaction(Faction.OfPlayer);
				return;
			}
			if (PlantUtility.GrowthSeasonNow(base.Position, base.Map))
			{
				float num = growthInt;
				bool flag = LifeStage == PlantLifeStage.Mature;
				growthInt += base.GrowthPerTick * 2000f;
				if (growthInt > 1f)
				{
					growthInt = 1f;
				}
				if (((!flag && LifeStage == PlantLifeStage.Mature) || (int)(num * 10f) != (int)(growthInt * 10f)) && CurrentlyCultivated())
				{
					base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
				}
			}
			if (!HasEnoughLightToGrow)
			{
				unlitTicks += 2000;
			}
			else
			{
				unlitTicks = 0;
			}
			ageInt += 2000;
			if (base.Dying)
			{
				Map map2 = base.Map;
				bool isCrop = base.IsCrop;
				bool harvestableNow = HarvestableNow;
				bool dyingBecauseExposedToLight = DyingBecauseExposedToLight;
				int num2 = Mathf.CeilToInt(CurrentDyingDamagePerTick * 2000f);
				TakeDamage(new DamageInfo(DamageDefOf.Rotting, num2));
				if (base.Destroyed)
				{
					if (isCrop && def.plant.Harvestable && MessagesRepeatAvoider.MessageShowAllowed("MessagePlantDiedOfRot-" + def.defName, 240f))
					{
						string key = (harvestableNow ? "MessagePlantDiedOfRot_LeftUnharvested" : ((!dyingBecauseExposedToLight) ? "MessagePlantDiedOfRot" : "MessagePlantDiedOfRot_ExposedToLight"));
						Messages.Message(key.Translate(GetCustomLabelNoCount(includeHp: false)), new TargetInfo(base.Position, map2), MessageTypeDefOf.NegativeEvent);
					}
					return;
				}
			}
			cachedLabelMouseover = null;
		}
	}
}
