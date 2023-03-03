using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{

    public class CompProperties_AdjustHediffs : CompProperties
    {
        public List<ResourceProperties> resourceSettings;

        public string disablePostUse;
    }
    public abstract class CompAdjustHediffs : ThingComp, IAdjustResource, IResourceStorage
    {
        public CompProperties_AdjustHediffs Props => (CompProperties_AdjustHediffs)this.props;
        public Thing Parent => this.parent;
        public virtual List<ResourceProperties> ResourceSettings => Props.resourceSettings;
        public string DisablePostUse => Props.disablePostUse;
        public bool IsStorageFor(ResourceProperties resourceProperties, out ResourceStorage resourceStorages)
        {
            resourceStorages = GetResourceStoragesFor(resourceProperties).FirstOrDefault()?.Item3;
            return resourceStorages != null;
        }

        private Dictionary<int, ResourceStorage> resourceStorages = new Dictionary<int, ResourceStorage>();
        public Dictionary<int, ResourceStorage> ResourceStorages
        {
            get
            {
                InitializeResourceStorages();
                return resourceStorages;
            }
        }

        public void InitializeResourceStorages()
        {
            if (resourceStorages is null)
            {
                resourceStorages = new Dictionary<int, ResourceStorage>();
            }
            if (Props.resourceSettings != null)
            {
                for (var i = 0; i < Props.resourceSettings.Count; i++)
                {
                    if (Props.resourceSettings[i].maxResourceStorageAmount > 0)
                    {
                        if (!resourceStorages.ContainsKey(i))
                        {
                            resourceStorages[i] = new ResourceStorage(Props.resourceSettings[i], this);
                            HRFLog.Message("Initializing storage for " + Props.resourceSettings[i].hediff);
                            if (Props.resourceSettings[i].initialResourceAmount != 0)
                            {
                                resourceStorages[i].ResourceAmount = Props.resourceSettings[i].initialResourceAmount;
                            }
                        }
                        if (resourceStorages[i].resourceProperties is null)
                        {
                            resourceStorages[i].resourceProperties = Props.resourceSettings[i];
                        }
                        if (resourceStorages[i].parent is null)
                        {
                            resourceStorages[i].parent = this;
                        }

                    }
                }
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (resourceStorages != null)
            {
                foreach (var storage in resourceStorages.Values)
                {
                    if (storage.resourceProperties.addHediffIfMissing)
                    {
                        var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(storage.resourceProperties.hediff);
                        if (hediff is null)
                        {
                            hediff = HediffMaker.MakeHediff(storage.resourceProperties.hediff, pawn);
                            pawn.health.AddHediff(hediff);
                        }
                    }
                }
            }
        }
        public IEnumerable<Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>> GetResourceStoragesFor(HediffResourceDef hediffDef)
        {
            var resourceStorages = ResourceStorages;
            if (Props.resourceSettings != null)
            {
                for (var i = 0; i < Props.resourceSettings.Count; i++)
                {
                    if (Props.resourceSettings[i].maxResourceStorageAmount > 0 && Props.resourceSettings[i].hediff == hediffDef)
                    {
                        yield return new Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>(this, Props.resourceSettings[i], resourceStorages[i]);
                    }
                }
            }
        }
        public IEnumerable<Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>> GetResourceStoragesFor(ResourceProperties resourceProperties)
        {
            var resourceStorages = ResourceStorages;
            if (Props.resourceSettings != null)
            {
                for (var i = 0; i < Props.resourceSettings.Count; i++)
                {
                    if (Props.resourceSettings[i].maxResourceStorageAmount > 0 && Props.resourceSettings[i].refillOnlyInnerStorage && Props.resourceSettings[i] == resourceProperties)
                    {
                        yield return new Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>(this, Props.resourceSettings[i], resourceStorages[i]);
                    }
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            var sb = new StringBuilder(base.CompInspectStringExtra());
            if (Props.resourceSettings != null)
            {
                for (var i = 0; i < Props.resourceSettings.Count; i++)
                {
                    var resourceProperties = Props.resourceSettings[i];
                    if (resourceProperties.maxResourceStorageAmount > 0)
                    {
                        foreach (var resourceStorage in GetResourceStoragesFor(resourceProperties).ToList())
                        {
                            if ((Find.TickManager.TicksGame - resourceStorage.Item3.lastChargedTick) <= 60)
                            {
                                sb.AppendLine("ART.StoredAmountCharged".Translate(resourceStorage.Item3.ResourceAmount, resourceProperties.maxResourceStorageAmount));
                            }
                            else
                            {
                                sb.AppendLine("ART.StoredAmount".Translate(resourceStorage.Item3.ResourceAmount, resourceProperties.maxResourceStorageAmount));
                            }
                        }
                    }

                    if (resourceProperties.disallowEquipIfHediffMissing || resourceProperties.disallowEquipWhenEmpty || resourceProperties.disableIfMissingHediff)
                    {
                        sb.AppendLine("ART.RequiresResource".Translate(resourceProperties.hediff.label));
                    }

                    if (Prefs.DevMode)
                    {
                        if (resourceProperties.minimumResourcePerUse != -1f)
                        {
                            sb.AppendLine("ART.MinimumResourcePerUse".Translate(resourceProperties.hediff.label, resourceProperties.minimumResourcePerUse));
                        }
                        if (resourceProperties.disableAboveResource != -1f)
                        {
                            sb.AppendLine("ART.WillBeDisabledWhenResourceAbove".Translate(resourceProperties.hediff.label, resourceProperties.disableAboveResource));
                        }

                        if (resourceProperties.resourcePerUse != 0)
                        {
                            sb.AppendLine("ART.ResourcePerUse".Translate(resourceProperties.hediff.label, -resourceProperties.resourcePerUse));
                        }
                        if (resourceProperties.resourcePerSecond != 0)
                        {
                            sb.AppendLine("ART.ResourcePerSecond".Translate(resourceProperties.hediff.label, resourceProperties.resourcePerSecond));
                        }
                        if (resourceProperties.maxResourceCapacityOffset != 0)
                        {
                            sb.AppendLine("ART.MaxResourceCapacityOffset".Translate(resourceProperties.hediff.label, resourceProperties.maxResourceCapacityOffset.ToStringWithSign()));
                        }
                        if (resourceProperties.refillOnlyInnerStorage)
                        {
                            sb.AppendLine("ART.IsBattery".Translate(resourceProperties.hediff.label, resourceProperties.maxResourceCapacityOffset));
                        }
                        if (resourceProperties.maxResourceStorageAmount != 0)
                        {
                            sb.AppendLine("ART.MaxResourceStorageAmount".Translate(resourceProperties.hediff.label, resourceProperties.maxResourceStorageAmount));
                        }
                    }
                }
            }
            return sb.ToString().TrimEndNewlines();
        }

        private Dictionary<HediffResource, HediffResouceDisable> postUseDelayTicks;
        public Dictionary<HediffResource, HediffResouceDisable> PostUseDelayTicks
        {
            get
            {
                if (postUseDelayTicks is null)
                {
                    postUseDelayTicks = new Dictionary<HediffResource, HediffResouceDisable>();
                }
                return postUseDelayTicks;
            }
        }

        public abstract Pawn PawnHost { get; }
        public bool TryGetQuality(out QualityCategory qc)
        {
            return this.parent.TryGetQuality(out qc);
        }

        public virtual void Drop()
        {

        }
        public void Register()
        {
            HRFManager.Instance.RegisterAdjuster(this);
            InitializeResourceStorages();
        }

        public void Deregister()
        {
            HRFManager.Instance.DeregisterAdjuster(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref postUseDelayTicks, "postUseDelayTicks", LookMode.Reference, LookMode.Deep, ref hediffResourceKeys, ref hediffResourceDisablesValues);
            Scribe_Collections.Look(ref resourceStorages, "resourceStorages", LookMode.Value, LookMode.Deep, ref intKeys, ref resourceStorageValues);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Register();
                InitializeResourceStorages();
            }
        }

        private List<HediffResource> hediffResourceKeys;
        private List<HediffResouceDisable> hediffResourceDisablesValues;

        private List<int> intKeys;
        private List<ResourceStorage> resourceStorageValues;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Register();
            if (!respawningAfterLoad)
            {
                TryForbidAfterPlacing();
            }

        }

        public void TryForbidAfterPlacing()
        {
            foreach (var thing in this.parent.Position.GetThingList(this.parent.Map))
            {
                var comp = thing.TryGetComp<CompBuildingStorageAdjustHediffs>();
                if (comp != null)
                {
                    foreach (var resourceSettings in comp.ResourceSettings)
                    {
                        foreach (var thisResourceSettings in this.ResourceSettings)
                        {
                            if (resourceSettings.hediff == thisResourceSettings.hediff && resourceSettings.forbidItemsWhenCharging)
                            {
                                if (comp.compPower != null && !comp.compPower.PowerOn)
                                {
                                    continue;
                                }
                                else
                                {
                                    this.parent.SetForbidden(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            Register();
        }

        public virtual void Notify_Removed()
        {
            Deregister();
        }

        public virtual void ResourceTick()
        {

        }

        public void Update()
        {

        }

        public ThingDef GetStuff()
        {
            return this.parent.Stuff;
        }

        public HediffResource GetResourceFor(ResourceProperties resourceProperties)
        {
            return PawnHost?.health?.hediffSet?.GetFirstHediffOfDef(resourceProperties.hediff) as HediffResource;
        }
    }
}
