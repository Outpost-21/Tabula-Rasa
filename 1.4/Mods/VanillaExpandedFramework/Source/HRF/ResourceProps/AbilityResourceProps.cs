using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class AbilityResourceProps : DefModExtension, IResourceProps
    {
        public List<ResourceProperties> resourceSettings;

        public List<ResourceProperties> targetResourceSettings;

        public List<ChargeSettings> chargeSettings;
        public List<ResourceProperties> ResourceSettings => resourceSettings;
        public List<ResourceProperties> TargetResourceSettings => targetResourceSettings;
        public List<ChargeSettings> ChargeSettings => chargeSettings;
    }
}
