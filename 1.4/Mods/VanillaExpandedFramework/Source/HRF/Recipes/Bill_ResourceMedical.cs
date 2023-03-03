using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;
using Verse.AI;

namespace HRF
{
    public class Bill_ResourceMedical : Bill_Medical
    {
        public Bill_ResourceMedical()
        {

        }
        public Bill_ResourceMedical(RecipeDef recipe, List<Thing> uniqueIngredients) : base(recipe, uniqueIngredients)
        {

        }
        public RecipeResourceIngredients Extension => this.recipe.GetModExtension<RecipeResourceIngredients>();
        public Dictionary<HediffResourceDef, float> consumedResources = new Dictionary<HediffResourceDef, float>();
        public override bool PawnAllowedToStartAnew(Pawn p)
        {
            return Bill_Resource.AllowedToStartAnew(p, Extension);
        }
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);
            consumedResources.Clear();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref consumedResources, "consumedResources", LookMode.Def, LookMode.Value, ref defKeys, ref floatValues);
        }

        private List<HediffResourceDef> defKeys;
        private List<float> floatValues;
    }
}
