using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Hediffs
{
    public class HediffComp_GeneticConversion : HediffComp
    {
        public HediffCompProperties_GeneticConversion Props => (HediffCompProperties_GeneticConversion)props;

        public Faction faction;

        public int finishingTick = -1;

        public override void CompPostMake()
        {
            base.CompPostMake();
            if(finishingTick < 0) { finishingTick = Props.tickRange.RandomInRange; }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Find.TickManager.TicksAbs > finishingTick) { BeginConversion(); }
        }

        public void BeginConversion()
        {
            if (Props.overwriteGenes)
            {
                Pawn.genes.ClearXenogenes();
            }
            if(Props.xenotype != null)
            {
                Pawn.genes.SetXenotypeDirect(Props.xenotype);
            }
            if (Props.convertPawn)
            {
                if(faction != null)
                {
                    Pawn.SetFaction(faction);
                }
                else if(Props.faction != null)
                {
                    Faction fac = Find.FactionManager.FirstFactionOfDef(Props.faction);
                    if(fac != null)
                    {
                        Pawn.SetFaction(fac);
                    }
                }
            }
        }
    }
}
