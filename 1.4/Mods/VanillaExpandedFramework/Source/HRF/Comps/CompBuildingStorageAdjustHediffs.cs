using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class CompProperties_BuildingStorageAdjustHediffs : CompProperties_AdjustHediffs
    {
        public CompProperties_BuildingStorageAdjustHediffs()
        {
            this.compClass = typeof(CompBuildingStorageAdjustHediffs);
        }
    }
    public class CompBuildingStorageAdjustHediffs : CompAdjustHediffs
    {
        public CompPowerTrader compPower;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            compPower = this.parent.TryGetComp<CompPowerTrader>();
        }
        public IEnumerable<CompAdjustHediffs> StoredItems
        {
            get
            {
                foreach (var cell in this.parent.OccupiedRect())
                {
                    foreach (var thing in cell.GetThingList(this.parent.Map))
                    {
                        if (thing != this.parent && thing.TryGetCompAdjustHediffs(out var comp))
                        {
                            yield return comp;
                        }
                    }
                }
            }
        }
        public override Pawn PawnHost => null;
        public override void ResourceTick()
        {
            base.ResourceTick();
            if (compPower != null && !compPower.PowerOn)
            {
                return;
            }
            var storedThingComps = StoredItems;
            foreach (var storedThingComp in storedThingComps)
            {
                foreach (var resourceProperties in Props.resourceSettings)
                {
                    float num = resourceProperties.GetResourceGain(this);
                    var storage = storedThingComp.GetResourceStoragesFor(resourceProperties.hediff).FirstOrDefault();
                    if (storage != null)
                    {
                        if (storage.Item3.ResourceAmount < storage.Item3.ResourceCapacity)
                        {
                            storage.Item3.ResourceAmount += num;
                            storage.Item3.lastChargedTick = Find.TickManager.TicksGame;
                        }

                        if (resourceProperties.unforbidWhenEmpty && storage.Item3.ResourceAmount <= 0 ||
                            resourceProperties.unforbidWhenFull && storage.Item3.ResourceAmount >= storage.Item3.ResourceCapacity)
                        {
                            storage.Item1.parent.SetForbidden(false);
                        }
                    }
                }
            }
        }
    }
}
