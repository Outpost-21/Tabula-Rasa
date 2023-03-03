using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class HediffResourcePolicy : IExposable
    {
        public HediffResourcePolicy()
        {

        }

        public Dictionary<HediffResourceDef, HediffResourceSatisfyPolicy> satisfyPolicies;
        public void ExposeData()
        {
            Scribe_Collections.Look(ref satisfyPolicies, "satisfyPolicies");
        }
    }
}
