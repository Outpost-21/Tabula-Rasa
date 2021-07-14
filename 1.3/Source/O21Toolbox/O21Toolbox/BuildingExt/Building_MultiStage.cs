using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class Building_MultiStage : Building
    {
        public DefModExt_MultiStage ext;

        public RecipeDef_MultiStage recipe = null;

        public int curWorkTime = -1;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref recipe, "recipe");

            Scribe_Values.Look(ref curWorkTime, "curWorkTime", -1);
        }


    }
}
