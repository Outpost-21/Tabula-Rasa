using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    public interface IAdjustResouceInArea : IAdjustResource
    {
        bool InRadiusFor(IntVec3 position, HediffResourceDef def);
        ResourceProperties GetFirstResourcePropertiesFor(HediffResourceDef def);
        float GetResourceCapacityGainFor(HediffResourceDef def);
    }
}