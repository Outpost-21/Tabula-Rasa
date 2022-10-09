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
    public class DesignatorSubCategoryDef : Def
    {
        public DesignationCategoryDef designationCategory;
        public bool enabled = false;
        public string iconPath;

        public Texture2D Icon => ContentFinder<Texture2D>.Get((!iconPath.NullOrEmpty()) ? iconPath : BaseContent.BadTexPath);
    }
}
