using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.BuildingExt
{
	public class Comp_PawnDeterrant : ThingComp
	{
		public CompProperties_PawnDeterrant Props => (CompProperties_PawnDeterrant)props;

        public int hashOffset = 0;

        public bool IsHashTick => (Find.TickManager.TicksGame + hashOffset) % Props.ticksBetweenRuns == 0;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            hashOffset = parent.thingIDNumber.HashOffset();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (IsHashTick)
            {
                DeterPawns();
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            DeterPawns();
        }

        public void DeterPawns()
        {
            IEnumerable<Pawn> pawns = from x
                                      in parent.Map.mapPawns.AllPawns
                                      where IsAffected(x)
                                      select x;
            if (!pawns.EnumerableNullOrEmpty())
            {
                foreach(Pawn pawn in pawns)
                {
                    if(pawn != null)
                    {
                        JobDef job;
                        if(pawn.jobs == null)
                        {
                            job = null;
                        }
                        else
                        {
                            Job curJob = pawn.jobs.curJob;
                            job = ((curJob != null) ? curJob.def : null);
                        }
                        if(job != JobDefOf.Flee)
                        {
                            if (pawn.Position.DistanceTo(parent.Position) < Props.radius)
                            {
                                Job newJob = new Job(JobDefOf.Flee, CellFinderLoose.GetFleeDest(pawn, parent.Map.listerThings.ThingsOfDef(parent.def), this.Props.minFleeDistance), parent.Position);
                                pawn.jobs.StartJob(newJob, JobCondition.InterruptOptional);
                            }
                        }
                    }
                }
            }
        }

        public bool IsAffected(Pawn pawn)
        {
            if (pawn.BodySize <= Props.maxBodySizeAffected)
            {
                if (!Props.raceWhitelist.NullOrEmpty())
                {
                    if (Props.raceWhitelist.Contains(pawn.def))
                    {
                        return true;
                    }
                    return false;
                }
                if (!Props.raceBlacklist.NullOrEmpty())
                {
                    if (!Props.raceBlacklist.Contains(pawn.def))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
