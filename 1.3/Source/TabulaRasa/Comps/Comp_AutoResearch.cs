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
    public class Comp_AutoResearch : ThingComp
    {
        public CompProperties_AutoResearch Props => (CompProperties_AutoResearch)props;

        private CompPowerTrader powerComp;

        private Pawn researchingPawn;

        private Pawn worstPawn;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look(ref researchingPawn, "researchingPawn");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            powerComp = parent.TryGetComp<CompPowerTrader>();
        }

        public override void CompTick()
        {
            base.CompTick();

            researchingPawn = GetBestResearcher();
            worstPawn = GetWorstResearcher();
            if (HasPower() && researchingPawn != null && Find.ResearchManager.currentProj != null)
            {
                float num = researchingPawn.GetStatValue(StatDefOf.ResearchSpeed, true);
                if (Props.totalPawnsAffectSpeed)
                {
                    num = (num * Props.researchSpeedFactor) + (Props.bonusPerPawn * parent.Map.mapPawns.ColonistsSpawnedCount);
                }
                else
                {
                    num *= this.Props.researchSpeedFactor;
                }
                Find.ResearchManager.ResearchPerformed(num, researchingPawn);
                researchingPawn.skills.Learn(SkillDefOf.Intellectual, 0.1f, false);
            }
        }

        public Pawn GetWorstResearcher()
        {
            if (Props.totalPawnsAffectSpeed)
            {
                Pawn result = null;
                IEnumerable<Pawn> enumerable;
                if (Props.pawnKind != null)
                {
                    enumerable = parent.Map.mapPawns.FreeColonistsSpawned.Where(p => p.def == Props.pawnKind.race);
                }
                else
                {
                    enumerable = parent.Map.mapPawns.FreeColonistsSpawned;
                }

                {
                    Pawn worstPawn = null;
                    float num = 0f;
                    foreach (Pawn curPawn in enumerable)
                    {
                        if (curPawn != worstPawn)
                        {
                            int num2 = curPawn.skills.skills.Find(s => s.def == SkillDefOf.Intellectual).Level;
                            if (worstPawn == null || num2 < num)
                            {
                                worstPawn = curPawn;
                                num = num2;
                            }
                        }
                    }
                    if (worstPawn != null)
                    {
                        result = worstPawn;
                    }
                }
                return result;
            }
            return null;
        }

        public Pawn GetBestResearcher()
        {
            Pawn result = null;
            IEnumerable<Pawn> enumerable;

            if (Props.pawnKind != null)
            {
                enumerable = parent.Map.mapPawns.FreeColonistsSpawned.Where(p => p.def == Props.pawnKind.race);
            }
            else
            {
                enumerable = parent.Map.mapPawns.FreeColonistsSpawned;
            }

            {
                Pawn bestPawn = null;
                float num = 0f;
                foreach (Pawn curPawn in enumerable)
                {
                    if (curPawn != bestPawn)
                    {
                        int num2 = curPawn.skills.skills.Find(s => s.def == SkillDefOf.Intellectual).Level;
                        if (bestPawn == null || num2 > num)
                        {
                            bestPawn = curPawn;
                            num = num2;
                        }
                    }
                }
                if (bestPawn != null)
                {
                    result = bestPawn;
                }
            }

            return result;
        }

        public bool HasPower()
        {
            if (Props.requiresPower && powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }
            return true;
        }
    }
}
