using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
    public class Bill_Resource : Bill_Production
    {
        public Bill_Resource()
        {

        }
        public Bill_Resource(RecipeDef recipe, Precept_ThingStyle precept = null) : base(recipe, precept)
        {

        }
        public RecipeResourceIngredients Extension => this.recipe.GetModExtension<RecipeResourceIngredients>();
        public Dictionary<HediffResourceDef, float> consumedResources = new Dictionary<HediffResourceDef, float>();
        public override bool PawnAllowedToStartAnew(Pawn p)
        {
            return AllowedToStartAnew(p, Extension);
        }
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);
            consumedResources.Clear();
        }

        public static bool AllowedToStartAnew(Pawn p, RecipeResourceIngredients extension)
        {
            foreach (var resourceCost in extension.recourseCostList)
            {
                var hediff = p.health.hediffSet.GetFirstHediffOfDef(resourceCost.resource) as HediffResource;
                if (hediff is null || resourceCost.cost > 0 && hediff.ResourceAmount < resourceCost.cost)
                {
                    return false;
                }
            }
            if (extension.recourseCostList.Any(x => x.cost < 0))
            {
                var allHediffs = p.health.hediffSet.hediffs.OfType<HediffResource>().Where(x => extension.recourseCostList.Any(y => y.resource == x.def)).ToList();
                if (allHediffs.Count != extension.recourseCostList.Count || allHediffs.All(x => x.ResourceAmount >= x.ResourceCapacity))
                {
                    return false;
                }
            }
            return true;
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
