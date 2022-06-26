using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    [DefOf]
    public static class TabulaRasaDefOf
    {
        static TabulaRasaDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TabulaRasaDefOf));
        }

        public static JobDef O21_UseTeleporter;
        public static JobDef O21_UseRecall;
    }
}
