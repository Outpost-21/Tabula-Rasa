using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    [DefOf]
    public class JobDefOf
    {
        public JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
        }

        public static JobDef O21_GatherSlotItem;
        public static JobDef O21_CastDeflectVerb;
    }
}
