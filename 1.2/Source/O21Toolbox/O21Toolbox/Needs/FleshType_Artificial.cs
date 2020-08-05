using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    [StaticConstructorOnStartup]
    static public class PawnExt
    {
        static public bool IsArtificialPawn(this FleshTypeDef fleshType)
        {
            return fleshType == ArtificialDefOf.Artificial;
        }
    }

    [DefOf]
    static public class ArtificialDefOf
    {
        static ArtificialDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ArtificialDefOf));
        }

        public static FleshTypeDef Artificial;
    }
}
