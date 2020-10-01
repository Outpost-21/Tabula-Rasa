using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AreaEffects
{
    public class Comp_AreaEffects : ThingComp
    {
        public int tickTimer = -1;

        public CompProperties_AreaEffects Props => (CompProperties_AreaEffects)this.props;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref tickTimer, "tickTimer", -1);
        }

        public override void CompTick()
        {
            base.CompTick();

            if(tickTimer < 0)
            {
                ApplyHediffsToPawns(GetNearbyPawns());
                tickTimer = Props.ticksBetweenRuns;
            }
            tickTimer--;
        }

        public void ApplyHediffsToPawns(List<Pawn> pawns)
        {
            if (pawns.NullOrEmpty())
            {
                return;
            }

            for (int i = 0; i < pawns.Count; i++)
            {
                foreach(HediffSeverityPairing hediff in Props.applyHediffs)
                {
                    if (pawns[i].health.hediffSet.HasHediff(hediff.hediff))
                    {
                        pawns[i].health.hediffSet.GetFirstHediffOfDef(hediff.hediff).Severity += hediff.severityIncrease;
                    }
                    else
                    {
                        Hediff newHediff = new Hediff();
                        newHediff.def = hediff.hediff;
                        newHediff.Severity = hediff.severityInitial;
                        pawns[i].health.AddHediff(newHediff);
                    }
                }
            }
        }

        public List<Pawn> GetNearbyPawns()
        {
            List<Pawn> pawns = new List<Pawn>();

            if (Props.roomBased)
            {
                Room room = parent.GetRoom();
                if(room != null && (!Props.roomRequiresRoof || room.PsychologicallyOutdoors))
                {
                    List<IntVec3> cells = room.Cells.ToList();
                    for (int i = 0; i < cells.Count; i++)
                    {
                        foreach(Thing thing in cells[i].GetThingList(parent.Map))
                        {
                            if(thing is Pawn && !pawns.Contains(thing))
                            {
                                pawns.Add(thing as Pawn);
                            }
                        }
                    }
                    return pawns;
                }
            }
            if(Props.radius > 0)
            {
                List<IntVec3> cells = GenRadial.RadialCellsAround(parent.Position, Props.radius, true).ToList();
                for (int i = 0; i < cells.Count; i++)
                {
                    foreach (Thing thing in cells[i].GetThingList(parent.Map))
                    {
                        if (thing is Pawn && !pawns.Contains(thing))
                        {
                            pawns.Add(thing as Pawn);
                        }
                    }
                }
                return pawns;
            }

            return pawns;
        }
    }
}
