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
    public class Recipe_Disassemble : RecipeWorker
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            yield return null;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            //Spawn extra butcher products.
            ButcherUtil.SpawnDrops(pawn, pawn.Position, pawn.Map);

            pawn.Destroy(DestroyMode.Vanish);
        }
    }
}
