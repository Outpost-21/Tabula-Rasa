using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Utility
{
    public static class ArtificialUtil
    {
        static public bool IsArtificialPawn(this FleshTypeDef fleshType)
        {
            return fleshType == TabulaRasaDefOf.TabulaRasa_Artificial;
        }
    }
}
