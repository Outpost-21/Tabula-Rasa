using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class CompProperties_PawnStorage : CompProperties
    {
        public CompProperties_PawnStorage()
        {
            this.compClass = typeof(Comp_PawnStorage);
        }

        /// <summary>
        /// Number of pawns storable on this.
        /// </summary>
        public int pawnLimit = 0;
    }
}
