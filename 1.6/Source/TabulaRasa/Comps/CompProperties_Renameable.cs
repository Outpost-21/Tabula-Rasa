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
    public class CompProperties_Renameable : CompProperties
    {
        public CompProperties_Renameable()
        {
            this.compClass = typeof(Comp_Renameable);
        }
    }
}
