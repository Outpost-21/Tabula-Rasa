using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    [StaticConstructorOnStartup]
    public static class AssignTraitCompToHumans
    {
        static AssignTraitCompToHumans()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Humanlike ?? false))
            {
                thingDef.comps.Add(new CompProperties_TraitsAdjustHediffs());
            }
        }
    }
    public class CompProperties_TraitsAdjustHediffs : CompProperties_AdjustHediffs
    {
        public CompProperties_TraitsAdjustHediffs()
        {
            this.compClass = typeof(CompTraitsAdjustHediffs);
        }
    }

    public class CompTraitsAdjustHediffs : CompAdjustHediffs
    {
        public static Dictionary<TraitDef, TraitsAdjustHediff> cachedModExtensions = new Dictionary<TraitDef, TraitsAdjustHediff>();
        public override List<ResourceProperties> ResourceSettings
        {
            get
            {
                var resourceSettings = new List<ResourceProperties>();
                foreach (var trait in PawnHost.story?.traits?.allTraits)
                {
                    if (!cachedModExtensions.TryGetValue(trait.def, out TraitsAdjustHediff traitAdjustOptions))
                    {
                        traitAdjustOptions = trait.def.GetModExtension<TraitsAdjustHediff>();
                        cachedModExtensions[trait.def] = traitAdjustOptions;
                    }
                    if (traitAdjustOptions?.resourceSettings != null)
                    {
                        resourceSettings.AddRange(traitAdjustOptions.resourceSettings);
                    }
                }
                return resourceSettings;
            }
        }

        public override Pawn PawnHost => this.parent as Pawn;

        public override void Drop()
        {

        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            this.Notify_Removed();
            base.PostDestroy(mode, previousMap);
        }
        public override void ResourceTick()
        {
            base.ResourceTick();
            var pawn = PawnHost;
            if (pawn != null && !pawn.Dead)
            {
                foreach (var resourceProperties in ResourceSettings)
                {
                    resourceProperties.AdjustResource(pawn, this, PostUseDelayTicks);
                }
            }
        }
    }
}
