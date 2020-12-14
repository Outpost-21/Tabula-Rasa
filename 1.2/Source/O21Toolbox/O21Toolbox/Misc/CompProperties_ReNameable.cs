using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Misc
{
    public class CompProperties_ReNameable : CompProperties
    {
        /// <summary>
        /// Adding this comp to a thing will allow the player to rename it at will.
        /// </summary>
        public CompProperties_ReNameable()
        {
            this.compClass = typeof(Comp_ReNameable);
        }
    }
}
