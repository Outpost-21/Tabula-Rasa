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
    public class Designator_CustomMenu : Designator
    {
        private DesignationSubCatDef[] selectedSubCats;

        private List<ThingDef> SelectedSubCatThings
        {
            get
            {
            }
        }

        protected void DoSubCategoryListing()
        {

        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            return false;
        }
    }
}
