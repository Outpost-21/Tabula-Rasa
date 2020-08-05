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
    public class Comp_SelfErase : ThingComp
    {
        public CompProperties_SelfErase Props => (CompProperties_SelfErase)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if(parent.Map == Current.Game.CurrentMap)
            {
                parent.Destroy();
            }
        }
    }
}
