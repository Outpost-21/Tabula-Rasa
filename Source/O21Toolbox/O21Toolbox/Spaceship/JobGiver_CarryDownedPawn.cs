﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace O21Toolbox.Spaceship
{
    public class JobGiver_CarryDownedPawn : ThinkNode_JobGiver
    {
        protected LocomotionUrgency defaultLocomotion = LocomotionUrgency.Jog;
        protected int jobMaxDuration = 999999;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_CarryDownedPawn jobGiver_CarryDownedPawn = (JobGiver_CarryDownedPawn)base.DeepCopy(resolve);
            return jobGiver_CarryDownedPawn;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            LordToil_EscortDownedPawn toil = pawn.GetLord().CurLordToil as LordToil_EscortDownedPawn;

            Pawn pawnToRescue = Util_Spaceship.GetNearestReachableDownedPawn(pawn);
            if (pawnToRescue != null)
            {
                return new Job(Util_JobDefOf.CarryDownedPawn, pawnToRescue, toil.Data.targetDestination)
                {
                    count = 1
                };
            }
            else
            {
                toil.Notify_RescueEnded();
            }
            return null;
        }
    }
}
