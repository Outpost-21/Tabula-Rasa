using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Terraformer
{
    public class TerrainCategoryDef : Def
    {
        public List<string> tags;

        public List<string> additionalTags;

        public List<string> excludedTags;
    }
}
