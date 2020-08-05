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
    public class CompProperties_AbilityItem : CompProperties
    {
        public List<AbilityDef> Abilities = new List<AbilityDef>();

        public Type AbilityUserClass;

        public CompProperties_AbilityItem()
        {
            compClass = typeof(Comp_AbilityItem);
            AbilityUserClass = typeof(GenericCompAbilityUser); // default
        }
    }
}
