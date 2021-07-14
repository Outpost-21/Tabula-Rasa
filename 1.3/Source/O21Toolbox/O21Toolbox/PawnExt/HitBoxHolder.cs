using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    class ThingWithComps_HitBox : ThingWithComps
    {
        public Pawn master = null;

        public override void Draw()
        {
        }

        public override void Tick()
        {
            base.Tick();
            CheckNeedsDestruction();
        }

        public void CheckNeedsDestruction()
        {
            if (master != null && this.Spawned)
            {
                if (!master.Spawned)
                {
                    this.Destroy(0);
                    return;
                }

            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.master, "master", false);
        }
    }
}
