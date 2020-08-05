using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class CompProperties_AbilityUser : CompProperties
    {
        public CompProperties_AbilityUser()
        {
            compClass = typeof(Comp_AbilityUser);
        }
    }
}
