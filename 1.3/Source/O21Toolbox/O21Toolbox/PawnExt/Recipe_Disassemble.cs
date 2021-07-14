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
            if (pawn.def.HasModExtension<DefModExt_ArtificialPawn>())
            {
                yield return null;
            }

            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            //Spawn extra butcher products.
            ButcherUtility.SpawnDrops(pawn, pawn.Position, pawn.Map);

            pawn.Kill(null);
        }
    }
}
