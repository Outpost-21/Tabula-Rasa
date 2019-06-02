using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace O21Toolbox.Spaceship
{
    public abstract class LordJob_SpaceshipBase : LordJob
    {
        public const int pawnExitedGoodwillImpact = 1;
        public const int pawnLostGoodwillImpact = -3;

        public IntVec3 targetDestination;

        public LordJob_SpaceshipBase()
        {

        }

        public LordJob_SpaceshipBase(IntVec3 targetDestination)
        {
            this.targetDestination = targetDestination;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec3>(ref this.targetDestination, "targetDestination");
        }

        public override StateGraph CreateGraph()
        {
            return null;
        }
    }
}
