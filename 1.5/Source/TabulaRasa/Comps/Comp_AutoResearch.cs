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

        public CompPowerTrader powerComp;

        public Pawn researchingPawnCached;

        public Pawn ResearchingPawn 
        { 
            get
            {
                if(researchingPawnCached == null)
                {
                    List<Pawn> potentialPawns = GetViablePawns();

                    if (!potentialPawns.NullOrEmpty())
                    {
                        Pawn best = null;
                        float num = 0f;
                        foreach (Pawn curPawn in potentialPawns)
                        {
                            if (curPawn != best)
                            {
                                int num2 = curPawn.skills.skills.Find(s => s.def == SkillDefOf.Intellectual).Level;
                                if (best == null || num2 > num)
                                {
                                    best = curPawn;
                                    num = num2;
                                }
                            }
                        }
                        if (best != null)
                        {
                            researchingPawnCached = best;
                        }
                    }
                }

                return researchingPawnCached;
            } 
        }

        public Pawn worstPawnCached;

        public Pawn WorstPawn
        {
            get
            {
                if (worstPawnCached == null)
                {
                    List<Pawn> potentialPawns = GetViablePawns();

                    if (Props.totalPawnsAffectSpeed)
                    {
                        if (!potentialPawns.NullOrEmpty())
                        {
                            Pawn worstPawn = null;
                            float num = 0f;
                            foreach (Pawn curPawn in potentialPawns)
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
                                worstPawnCached = worstPawn;
                            }
                        }
                    }
                }

                return worstPawnCached;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            powerComp = parent.TryGetComp<CompPowerTrader>();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (HasPower() && ResearchingPawn != null && Find.ResearchManager.currentProj != null)
            {
                float num = ResearchingPawn.GetStatValue(StatDefOf.ResearchSpeed, true);
                if (Props.totalPawnsAffectSpeed)
                {
                    num = (num * Props.researchSpeedFactor) + (Props.bonusPerPawn * parent.Map.mapPawns.ColonistsSpawnedCount);
                }
                else
                {
                    num *= this.Props.researchSpeedFactor;
                }
                Find.ResearchManager.ResearchPerformed(num, ResearchingPawn);
                ResearchingPawn.skills.Learn(SkillDefOf.Intellectual, 0.1f, false);
            }
        }

        public bool HasPower()
        {
            if (Props.requiresPower && powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }
            return true;
        }

        public List<Pawn> GetViablePawns()
        {
            List<Pawn> potentialPawns = new List<Pawn>();
            if (Props.pawnKind != PawnKindDefOf.Colonist)
            {
                foreach (Pawn p in parent.Map.mapPawns.FreeColonistsSpawned)
                {
                    if (p.def == Props.pawnKind.race)
                    {
                        potentialPawns.Add(p);
                    }
                }
            }
            else if (Props.xenotype != XenotypeDefOf.Baseliner)
            {
                foreach (Pawn p in parent.Map.mapPawns.FreeColonistsSpawned)
                {
                    if (p.genes.Xenotype == Props.xenotype)
                    {
                        potentialPawns.Add(p);
                    }
                }
            }
            else
            {
                potentialPawns = parent.Map.mapPawns.FreeColonistsSpawned;
            }
            return potentialPawns;
        }
    }
}
