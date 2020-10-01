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
    public static class MemoryDefOf
    {
        static MemoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MemoryDefOf));
        }

        public static ThoughtDef O21_SawFullMoon;
    }
}
