using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TabulaRasa
{
    public class JobDriver_PlaySounds : JobDriver_WatchBuilding
    {
        public override void WatchTickAction()
        {
            DefModExt_Sounds ext = this.job.def.GetModExtension<DefModExt_Sounds>();

            if (this.pawn.IsHashIntervalTick(400 + Rand.Range(0, 100)) && !ext.soundDefs.NullOrEmpty())
            {
                ext.soundDefs.RandomElement().PlayOneShot(new TargetInfo(this.pawn.Position, this.pawn.Map, false));
            }
            base.WatchTickAction();
        }
    }
}
