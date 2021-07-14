using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Research
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
                    Pawn pawn2 = null;
                    float num = 0f;
                    foreach (Pawn pawn3 in enumerable)
                    {
                        if (pawn3 != pawn2)
                        {
                            int num2 = pawn3.skills.skills.Find(s => s.def == SkillDefOf.Intellectual).Level;
                            if (pawn2 == null || num2 < num)
                            {
                                pawn2 = pawn3;
                                num = num2;
                            }
                        }
                    }
                    if (pawn2 != null)
                    {
                        result = pawn2;
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
                Pawn pawn2 = null;
                float num = 0f;
                foreach (Pawn pawn3 in enumerable)
                {
                    if (pawn3 != pawn2)
                    {
                        int num2 = pawn3.skills.skills.Find(s => s.def == SkillDefOf.Intellectual).Level;
                        if (pawn2 == null || num2 > num)
                        {
                            pawn2 = pawn3;
                            num = num2;
                        }
                    }
                }
                if (pawn2 != null)
                {
                    result = pawn2;
                }
            }

            return result;
        }

        public bool HasPower()
        {
            if(Props.requiresPower && powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }
            return true;
        }
    }
}
