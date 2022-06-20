using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
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

            if (tickTimer < 0)
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
                foreach (HediffSeverityPairing hediff in Props.applyHediffs)
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
                if (room != null && (!Props.roomRequiresRoof || room.PsychologicallyOutdoors))
                {
                    List<IntVec3> cells = room.Cells.ToList();
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
            }
            if (Props.radius > 0)
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

    public class CompProperties_AreaEffects : CompProperties
    {
        /// <summary>
        /// If true the thing will look for pawns in the room to apply to, ignoring the radius. If false it will use the radius.
        /// If true and no room is detected, it will default to radius, but if the radius is not defined it will do nothing.
        /// </summary>
        public bool roomBased = true;

        /// <summary>
        /// If true, and roomBased is true, room detection will check for a roof.
        /// </summary>
        public bool roomRequiresRoof = true;

        /// <summary>
        /// Radius to apply effect to.
        /// </summary>
        public int radius = 0;

        /// <summary>
        /// Hediffs to apply while pawns are within the same room or radius.
        /// </summary>
        public List<HediffSeverityPairing> applyHediffs = new List<HediffSeverityPairing>();

        /// <summary>
        /// Adjustable time between each running of the code.
        /// </summary>
        public int ticksBetweenRuns = 250;
    }

    public class HediffSeverityPairing
    {
        public HediffDef hediff;

        /// <summary>
        /// Initial severity when the hediffs are applied.
        /// </summary>
        public float severityInitial = 0.01f;

        /// <summary>
        /// Severity increase if the pawn already has the hediff.
        /// </summary>
        public float severityIncrease = 0.01f;
    }
}
