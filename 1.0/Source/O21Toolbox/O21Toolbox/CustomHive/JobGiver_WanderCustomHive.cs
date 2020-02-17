using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.CustomHive
{
    internal class JobGiver_WanderCustomHive : JobGiver_Wander
    {
        public JobGiver_WanderCustomHive()
        {
            this.wanderRadius = 7.5f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }
        
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            CustomHive customHive = pawn.mindState.duty.focus.Thing as CustomHive;
            bool flag = customHive == null || !customHive.Spawned;
            IntVec3 position;
            if (flag)
            {
                position = pawn.Position;
            }
            else
            {
                position = customHive.Position;
            }
            return position;
        }
    }
}
