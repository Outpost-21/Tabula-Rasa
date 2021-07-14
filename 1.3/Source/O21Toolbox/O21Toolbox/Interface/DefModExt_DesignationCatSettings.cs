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
    public class DefModExt_DesignationCatSettings : DefModExtension
    {
        public DesignationSubCatDef category;

        public List<DesignationSubCatDef> categories;

        public bool hidden = false;
    }
}
