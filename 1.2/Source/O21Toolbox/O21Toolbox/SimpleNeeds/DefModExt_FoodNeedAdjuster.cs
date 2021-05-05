using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.SimpleNeeds
{
    public class DefModExt_FoodNeedAdjuster : DefModExtension
    {
        public string newLabel = "Energy";

        public bool disableFoodNeed = false;

        public bool dieOnEmpty = false;

        public HediffDef malnutritionReplacer;
    }
}
