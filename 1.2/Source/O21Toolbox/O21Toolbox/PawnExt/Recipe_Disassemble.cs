using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Needs;
using O21Toolbox.Utility;

namespace O21Toolbox.ArtificialPawn
{
    /// <summary>
    /// Safely disassembles Droids.
    /// </summary>
    public class Recipe_Disassemble : RecipeWorker
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            //If damaged, have option to apply.
            if (pawn.def.HasModExtension<ArtificialPawnProperties>())
            {
                yield return null;
            }

            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            //Set the power Need and EnergyTrackerComp to 0.
            Need_Energy needEnergy = pawn.needs.TryGetNeed<Need_Energy>();
            Comp_EnergyTracker energyTrackerComp = pawn.TryGetComp<Comp_EnergyTracker>();

            if (needEnergy != null)
            {
                needEnergy.CurLevelPercentage = 0f;
            }

            if (energyTrackerComp != null)
            {
                energyTrackerComp.energy = 0f;
            }

            //Spawn extra butcher products.
            ButcherUtility.SpawnDrops(pawn, pawn.Position, pawn.Map);

            pawn.Kill(null);
        }
    }
}
