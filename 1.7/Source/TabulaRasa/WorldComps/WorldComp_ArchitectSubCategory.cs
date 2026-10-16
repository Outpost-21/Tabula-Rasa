using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class WorldComp_ArchitectSubCategory : WorldComponent
    {
        public static Dictionary<DesignationCategoryDef, DesignatorSubCategoryDef> SelectedSubCategory = new Dictionary<DesignationCategoryDef, DesignatorSubCategoryDef>();

        public WorldComp_ArchitectSubCategory(World world) : base(world)
        {
        }

        public static void SetSubCategoryForDesingationCat(DesignationCategoryDef mainCat, DesignatorSubCategoryDef subCat)
        {
            if(subCat == null)
            {
                if (SelectedSubCategory.ContainsKey(mainCat))
                {
                    SelectedSubCategory.Remove(mainCat);
                }
                return;
            }
            if (SelectedSubCategory.ContainsKey(mainCat))
            {
                SelectedSubCategory[mainCat] = subCat;
            }
            else
            {
                SelectedSubCategory.Add(mainCat, subCat);
            }
        }
    }
}
