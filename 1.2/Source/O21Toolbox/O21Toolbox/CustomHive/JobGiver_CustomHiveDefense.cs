using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomHive
{
    internal class JobGiver_CustomHiveDefense : JobGiver_AIFightEnemies
    {
        protected override IntVec3 GetFlagPosition(Pawn pawn)
        {
            CustomHive customHive = pawn.mindState.duty.focus.Thing as CustomHive;
            bool flag = customHive != null && customHive.Spawned;
            IntVec3 position;
            if (flag)
            {
                position = customHive.Position;
            }
            else
            {
                position = pawn.Position;
            }
            return position;
        }
        
        protected override float GetFlagRadius(Pawn pawn)
        {
            return pawn.mindState.duty.radius;
        }
    }
}
