using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class VerbResourceProps : VerbProperties, IResourceProps
    {
        public List<ResourceProperties> resourceSettings;

        public List<ResourceProperties> targetResourceSettings;

        public List<ChargeSettings> chargeSettings;
        public List<ResourceProperties> ResourceSettings => resourceSettings;
        public List<ResourceProperties> TargetResourceSettings => targetResourceSettings;
        public List<ChargeSettings> ChargeSettings => chargeSettings;
    }
}
