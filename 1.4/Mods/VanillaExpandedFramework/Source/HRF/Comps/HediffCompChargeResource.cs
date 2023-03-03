using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class HediffCompProperties_ChargeResource : HediffCompProperties
    {
        public HediffCompProperties_ChargeResource()
        {
            this.compClass = typeof(HediffCompChargeResource);
        }
    }
    public class HediffCompChargeResource : HediffComp, IChargeResource
    {
        private Dictionary<Projectile, ChargeResources> projectilesWithChargedResource = new Dictionary<Projectile, ChargeResources>();
        public Dictionary<Projectile, ChargeResources> ProjectilesWithChargedResource
        {
            get
            {
                if (projectilesWithChargedResource is null)
                {
                    projectilesWithChargedResource = new Dictionary<Projectile, ChargeResources>();
                }
                return projectilesWithChargedResource;
            }
        }
        public HediffCompProperties_ChargeResource Props => (HediffCompProperties_ChargeResource)this.props;
        public override void CompExposeData()
        {
            base.CompExposeData();
            projectilesWithChargedResource?.RemoveAll(x => x.Key.DestroyedOrNull());
            Scribe_Collections.Look(ref projectilesWithChargedResource, "projectilesWithChargedResource", LookMode.Reference, LookMode.Deep, ref projectileValues, ref projectileVlaues);
        }

        private List<Projectile> projectileValues;
        private List<ChargeResources> projectileVlaues;
    }
}
