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
    public class Comp_AlienBodyCorrection : ThingComp
    {
        public CompProperties_AlienBodyCorrection Props => (CompProperties_AlienBodyCorrection)props;

        public bool completed = false;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref completed, "completed", false);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!completed)
            {
                CorrectBodyNow();
            }
        }

        public void CorrectBodyNow()
        {
            Pawn pawn = this.parent as Pawn;

            if(pawn.DevelopmentalStage == DevelopmentalStage.Adult)
            {
                if (pawn.gender == Gender.Male)
                {
                    if (!Props.maleBodyTypes.Contains(pawn.story.bodyType))
                    {
                        pawn.story.bodyType = Props.maleBodyTypes.RandomElement();
                    }
                }
                else if (pawn.gender == Gender.Female)
                {
                    if (!Props.femaleBodyTypes.Contains(pawn.story.bodyType))
                    {
                        pawn.story.bodyType = Props.femaleBodyTypes.RandomElement();
                    }
                }
            }

            completed = true;
        }
    }
}
