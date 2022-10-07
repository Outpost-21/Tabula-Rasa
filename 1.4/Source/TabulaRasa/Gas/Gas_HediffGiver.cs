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
    public class Gas_HediffGiver : Gas
    {
        public const int tickRate = 30;
        public Dictionary<Pawn, int> affectedPawns = new Dictionary<Pawn, int>();
        public List<Pawn> pawnKeys;
        public List<int> intValues;

        public DefModExt_GasHediffGiver modExt => def.GetModExtension<DefModExt_GasHediffGiver>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref affectedPawns, "affectedPawns", LookMode.Reference, LookMode.Value, ref pawnKeys, ref intValues);
        }

        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(tickRate))
            {
                if (affectedPawns == null)
                {
                    affectedPawns = new Dictionary<Pawn, int>();
                }
                List<Pawn> touchedPawns = new List<Pawn>();
                foreach (Thing thing in GenRadial.RadialDistinctThingsAround(this.Position, this.Map, modExt.radius, true))
                {
                    Pawn pawn = thing as Pawn;
                    if (pawn != null)
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
                    if (affectedPawns[pawn] >= modExt.ticksBeforeApply)
                    {
                        Dictionary<Pawn, int> dictionary = this.affectedPawns;
                        dictionary[pawn] -= modExt.ticksBeforeApply;
                        if (modExt.checkToxicSensitivity)
                        {
                            HealthUtility.AdjustSeverity(pawn, modExt.hediffDef, (modExt.adjustSeverity * pawn.GetStatValue(StatDefOf.ToxicEnvironmentResistance)));
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(pawn, modExt.hediffDef, modExt.adjustSeverity);
                        }
                    }
                }
            }
        }
    }
}
