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
    public class Comp_AdvancedHatcher : ThingComp
    {
        public CompProperties_AdvancedHatcher Props => (CompProperties_AdvancedHatcher)props;

        public float gestateProgress;

        public Pawn hatcheeParent;

        public Pawn otherParent;

        public Faction hatcheeFaction = Faction.OfPlayer;

        public CompTemperatureRuinable FreezerComp => parent.TryGetComp<CompTemperatureRuinable>();

        public bool TemperatureDamaged => FreezerComp != null && FreezerComp.Ruined;

        public override void CompTick()
        {
            if (!TemperatureDamaged)
            {
                float num = 1f / (Props.daysToHatch * 60000f);
                gestateProgress += num;
                if(gestateProgress >= 1f)
                {
                    Hatch();
                }
            }
        }

        public void Hatch()
        {
            for (int i = 0; i < parent.stackCount; i++)
            {
                PawnKindDef chosenDef = null;
                if (Props.pawnKind != null)
                {
                    chosenDef = Props.pawnKind;
                }
                if (!Props.pawnKinds.NullOrEmpty())
                {
                    chosenDef = Props.pawnKinds.RandomElement();
                }

                if (chosenDef != null)
                {
                    PawnGenerationRequest request = new PawnGenerationRequest(chosenDef, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, fixedBiologicalAge: 0f);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent))
                    {
                        if(pawn != null)
                        {
                            if(hatcheeParent != null)
                            {
                                if(pawn.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
                                {
                                    pawn.playerSettings.allowedAreas = hatcheeParent.playerSettings.allowedAreas;
                                }
                                if (pawn.RaceProps.IsFlesh)
                                {
                                    pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
                                }
                            }
                            if(otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && pawn.RaceProps.IsFlesh)
                            {
                                pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
                            }
                        }
                        if (parent.Spawned && (pawn == null || pawn.RaceProps.IsFlesh))
                        {
                            FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid);
                        }
                    }
                    else
                    {
                        Find.WorldPawns.PassToWorld(pawn, RimWorld.Planet.PawnDiscardDecideMode.Discard);
                    }
                }
                else
                {
                    LogUtil.LogError($"Failed to hatch egg of def: {parent.def.defName} due to no viable pawnKind or pawnKinds listed. Destroying item to prevent further errors.");
                }
                parent.Destroy();
            }
        }

        public override void PreAbsorbStack(Thing otherStack, int count)
        {
            float t = (float)count / (float)(parent.stackCount + count);
            float b = ((ThingWithComps)otherStack).GetComp<Comp_AdvancedHatcher>().gestateProgress;
            gestateProgress = Mathf.Lerp(gestateProgress, b, t);
        }

        public override void PostSplitOff(Thing piece)
        {
            Comp_AdvancedHatcher comp = ((ThingWithComps)piece).GetComp<Comp_AdvancedHatcher>();
            comp.gestateProgress = gestateProgress;
            comp.hatcheeParent = hatcheeParent;
            comp.otherParent = otherParent;
            comp.hatcheeFaction = hatcheeFaction;
        }

        public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PrePreTraded(action, playerNegotiator, trader);
            switch (action)
            {
                case TradeAction.PlayerBuys:
                    hatcheeFaction = Faction.OfPlayer;
                    break;
                case TradeAction.PlayerSells:
                    hatcheeFaction = trader.Faction;
                    break;
            }
        }

        public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
        {
            base.PostPostGeneratedForTrader(trader, forTile, forFaction);
            hatcheeFaction = forFaction;
        }

        public override string CompInspectStringExtra()
        {
            if (!TemperatureDamaged)
            {
                return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate((Props.daysToHatch * (1f - gestateProgress)).ToString("F1"));
            }
            return null;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f);
            Scribe_References.Look(ref hatcheeParent, "hatcheeParent");
            Scribe_References.Look(ref otherParent, "otherParent");
            Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
        }
    }
}
