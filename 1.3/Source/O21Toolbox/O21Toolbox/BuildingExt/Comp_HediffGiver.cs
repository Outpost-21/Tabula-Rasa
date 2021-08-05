using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class Comp_HediffGiver : ThingComp
    {
        public const int tickRate = 30;
        public Dictionary<Pawn, int> affectedPawns = new Dictionary<Pawn, int>();
        public List<Pawn> pawnKeys;
        public List<int> intValues;

        public CompProperties_HediffGiver Props => (CompProperties_HediffGiver)props;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref affectedPawns, "affectedPawns", LookMode.Reference, LookMode.Value, ref pawnKeys, ref intValues);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.IsHashIntervalTick(tickRate))
            {
                if(affectedPawns == null)
                {
                    affectedPawns = new Dictionary<Pawn, int>();
                }
                List<Pawn> touchedPawns = new List<Pawn>();
                foreach (Thing thing in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, Props.radius, true))
                {
                    Pawn pawn = (Pawn)thing;
                    if(pawn != null)
                    {
                        touchedPawns.Add(pawn);
                        if (affectedPawns.ContainsKey(pawn))
                        {
                            Dictionary<Pawn, int> dict = affectedPawns;
                            dict[pawn] += tickRate;
                        }
                        else
                        {
                            affectedPawns[pawn] = tickRate;
                        }
                    }
                }
                affectedPawns.RemoveAll((KeyValuePair<Pawn, int> x) => !touchedPawns.Contains(x.Key));
                foreach (Pawn pawn in affectedPawns.Keys.ToList<Pawn>())
                {
                    if(affectedPawns[pawn] >= Props.ticksBeforeApply)
                    {
                        Dictionary<Pawn, int> dictionary = this.affectedPawns;
                        dictionary[pawn] -= Props.ticksBeforeApply;
                        HealthUtility.AdjustSeverity(pawn, Props.hediffDef, Props.adjustSeverity);
                    }
                }
            }
        }
    }
}
