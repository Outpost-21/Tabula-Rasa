using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Interface
{
    [StaticConstructorOnStartup]
    public static class SubCategoryCache
    {
        public static Dictionary<DesignationSubCatDef, ThingDef> subCategories = new Dictionary<DesignationSubCatDef, ThingDef>();

        static SubCategoryCache()
        {
            OrganiseCategories(subCategories);
        }

        public static void OrganiseCategories(Dictionary<DesignationSubCatDef, ThingDef> subCategories)
        {
            List<ThingDefs>
        }
    }
}
