using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class RecipeOutcome
    {
        public List<ResourceCost> costs;
        public List<ThingDefCountClass> products;
        public string topLeftMessageSuccessKey;
        public string letterTitleSuccessKey;
        public string letterDescriptionSuccessKey;
        public SoundDef soundDef;
        public void Consume(Pawn pawn)
        {
            foreach (var cost in costs)
            {
                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(cost.resource) as HediffResource;
                hediff.ChangeResourceAmount(-cost.cost);
            }
        }
    }
    public class RecipeOutcomes : DefModExtension
    {
        public List<RecipeOutcome> recipeOutcomes;
    }
}
