using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class Comp_Resurrection : ThingComp
    {
        public CompProperties_Resurrection Props => (CompProperties_Resurrection)props;

        public bool failedResurrection = false;

        public int tickToRes = -1;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Corpse corpse = parent as Corpse;
                if (!Rand.Chance(Props.chanceToResurrect))
                {
                    failedResurrection = true;
                }
                else if(corpse.InnerPawn.health.hediffSet.GetNotMissingParts().Where(p => p.def == Props.requiredBodyPart).EnumerableNullOrEmpty())
                {
                    failedResurrection = true;
                }
                else
                {
                    tickToRes = Current.Game.tickManager.TicksGame + Props.ticksToResurrect.RandomInRange;
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!failedResurrection)
            {
                if(tickToRes >= Current.Game.tickManager.TicksGame)
                {
                    Resurrect();
                }
            }
        }

        public void Resurrect()
        {

        }
    }
}
