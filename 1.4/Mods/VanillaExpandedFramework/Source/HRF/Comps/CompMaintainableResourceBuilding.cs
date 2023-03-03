using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{

    public class CompMaintainableResourceBuilding : ThingComp
    {
		private CompThingInUse compFacilityInUse;

		public static Dictionary<Map, HashSet<Thing>> maintainables = new Dictionary<Map, HashSet<Thing>>();
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			compFacilityInUse = this.parent.TryGetComp<CompThingInUse>();
			var map = this.parent.Map;
			if (maintainables.ContainsKey(map))
            {
				maintainables[map].Add(this.parent);
			}
			else
            {
				maintainables[map] = new HashSet<Thing> { this.parent };
			}
		}

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
			if (maintainables.ContainsKey(previousMap))
            {
				maintainables[previousMap].Remove(this.parent);
			}
            base.PostDestroy(mode, previousMap);
        }

        public int ticksSinceMaintain;
		public CompProperties_Maintainable Props => (CompProperties_Maintainable)props;
		public MaintainableStage CurStage
		{
			get
			{
				if (ticksSinceMaintain < Props.ticksHealthy)
				{
					return MaintainableStage.Healthy;
				}
				if (ticksSinceMaintain < Props.ticksHealthy + Props.ticksNeedsMaintenance)
				{
					return MaintainableStage.NeedsMaintenance;
				}
				return MaintainableStage.Damaging;
			}
		}

		public bool CanMaintain(Pawn pawn, out string failReason)
        {
			if (compFacilityInUse != null)
            {
				foreach (var useProps in compFacilityInUse.Props.useProperties)
                {
					if (useProps.resourceOnComplete != -1)
                    {
						if (!compFacilityInUse.UseIsEnabled(useProps))
                        {
							failReason = useProps.cannotUseMessageKey;
							return false;
                        }
						var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(useProps.hediff) as HediffResource;
						if (hediff is null || hediff.ResourceAmount < -useProps.resourceOnComplete)
                        {
							failReason = useProps.cannotUseMessageKey;
							return false;
                        }
					}
                }
            }
			failReason = "";
			return true;
        }

		private bool Active => true;

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref ticksSinceMaintain, "ticksSinceMaintain", 0);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (Active)
			{
				ticksSinceMaintain++;
				if (Find.TickManager.TicksGame % 250 == 0)
				{
					CheckTakeDamage();
				}
			}
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			if (Active)
			{
				ticksSinceMaintain += 250;
				CheckTakeDamage();
			}
		}

		private void CheckTakeDamage()
		{
			if (CurStage == MaintainableStage.Damaging)
			{
				parent.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, Props.damagePerTickRare));
			}
		}

		public void Maintained(Pawn maintainer)
		{
			ticksSinceMaintain = 0;
			if (compFacilityInUse != null)
			{
				foreach (var useProps in compFacilityInUse.Props.useProperties)
				{
					if (useProps.resourceOnComplete != -1)
					{
						var hediff = maintainer.health.hediffSet.GetFirstHediffOfDef(useProps.hediff) as HediffResource;
						hediff.ChangeResourceAmount(useProps.resourceOnComplete);
					}
				}
			}
			this.parent.HitPoints = this.parent.MaxHitPoints;
		}

		public override string CompInspectStringExtra()
		{
			switch (CurStage)
			{
				case MaintainableStage.NeedsMaintenance:
					return "DueForMaintenance".Translate();
				case MaintainableStage.Damaging:
					return "DeterioratingDueToLackOfMaintenance".Translate();
				default:
					return null;
			}
		}
	}
}
