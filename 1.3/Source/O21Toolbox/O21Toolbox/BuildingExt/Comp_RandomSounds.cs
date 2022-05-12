using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace O21Toolbox
{
    public class Comp_RandomSounds : ThingComp
    {
        public CompProperties_RandomSounds Props => (CompProperties_RandomSounds)props;

        public int nextTick = -1;

        public CompPowerTrader PowerComp;

        public bool IsPowered => (PowerComp == null || PowerComp.PowerOn);

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref nextTick, "nextTick", -1);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            PowerComp = parent.GetComp<CompPowerTrader>();
        }

        public void CheckTick()
        {
            if (IsPowered && Find.TickManager.TicksGame > nextTick)
            {
                PlayRandomSound();
                nextTick = Find.TickManager.TicksGame + Props.tickRange.RandomInRange;
            }
        }

        public void PlayRandomSound()
        {
            Props.soundDefs.RandomElement().PlayOneShot(parent);
        }

        public override void CompTick()
        {
            base.CompTick();
            CheckTick();
        }

        public override void CompTickLong()
        {
            base.CompTickLong();
            CheckTick();
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            CheckTick();
        }
    }
}
