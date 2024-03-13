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
    public class CompProperties_TraitsOverTime : CompProperties
    {
        public CompProperties_TraitsOverTime()
        {
            this.compClass = typeof(Comp_TraitsOverTime);
        }

        public int maxTraits = 3;

        public IntRange timeBetweenTraits = new IntRange(30000, 60000);

        public List<TraitEntryAdvanced> traitWhitelist = new List<TraitEntryAdvanced>();

        public List<TraitEntryAdvanced> traitBlacklist = new List<TraitEntryAdvanced>();
    }
}
