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
    public static class Toils_Extra
    {
        public static Toil DestroyThing(TargetIndex ind)
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Pawn actor = toil.actor;
                Thing thing = actor.jobs.curJob.GetTarget(ind).Thing;
                bool flag = !thing.Destroyed;
                if (flag)
                {
                    thing.Destroy(0);
                }
            };
            return toil;
        }
    }
}
