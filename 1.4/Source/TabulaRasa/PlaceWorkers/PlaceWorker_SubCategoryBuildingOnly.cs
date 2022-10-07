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
    public class PlaceWorker_SubCategoryBuildingOnly : PlaceWorker
    {
        public DesignationCategoryDef CurrentCategory => Find.WindowStack.WindowOfType<MainTabWindow_Architect>()?.selectedDesPanel?.def ?? null;

        public override bool IsBuildDesignatorVisible(BuildableDef def)
        {
            if (CurrentCategory != null && (WorldComp_ArchitectSubCategory.SelectedSubCategory.ContainsKey(CurrentCategory) && WorldComp_ArchitectSubCategory.SelectedSubCategory[CurrentCategory].buildings.Contains(def)))
            {
                return true;
            }
            return false;
        }
    }
}
