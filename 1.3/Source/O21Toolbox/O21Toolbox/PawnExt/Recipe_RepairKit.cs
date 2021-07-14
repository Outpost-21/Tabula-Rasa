using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Needs;

namespace O21Toolbox.ArtificialPawn
{
    public class Recipe_RepairKit : RecipeWorker
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            //If damaged, have option to apply.
            if (pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
            {
                if (
                    pawn.health.hediffSet.BleedRateTotal > 0f ||
                    pawn.health.summaryHealth.SummaryHealthPercent < 1f ||
                    pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count > 0 ||
                    pawn.health.hediffSet.
                    hediffs.Any(hediff => hediff.def.HasModExtension<DefModExt_ArtificialPawn>() && hediff.CurStage.becomeVisible == true))
                    yield return null;
            }
            else
            {
                if (pawn.health.hediffSet.BleedRateTotal > 0f)
                    yield return null;
            }


            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            //Remove Coolant Loss
            //Hediff coolantLoss = pawn.health.hediffSet.GetFirstHediffOfDef(pawn.def.GetModExtension<ArtificialPawnProperties>().coolantLoss);
            //if (coolantLoss != null)
            //    pawn.health.RemoveHediff(coolantLoss);

            //Restore body to full condition on mechanical pawns.
            if (pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
            {
                //Make list of Hediffs to remove.
                List<Hediff> hediffsToRemove = new List<Hediff>();
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff is Hediff_MissingPart || hediff is Hediff_Injury || hediff.def.HasModExtension<DefModExt_ArtificialPawn>())
                        hediffsToRemove.Add(hediff);
                }

                //Remove all of them.
                foreach (Hediff hediff in hediffsToRemove)
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }
}
