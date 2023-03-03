using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public interface IResourceProps
    {
        List<ResourceProperties> ResourceSettings { get; }

        List<ResourceProperties> TargetResourceSettings { get; }

        List<ChargeSettings> ChargeSettings { get; }
    }
}
