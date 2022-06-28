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
    public class Comp_RecipesFromFacilities : CompAffectedByFacilities
    {
        public CompProperties_RecipesFromFacilities Props => (CompProperties_RecipesFromFacilities)props;

        public static void AddRecipe(CompAffectedByFacilities facilityComp)
        {
            if(facilityComp.parent.TryGetComp<Comp_RecipesFromFacilities>() != null)
            {
                Comp_RecipesFromFacilities recipeFacilityComp = facilityComp.parent.TryGetComp<Comp_RecipesFromFacilities>();
                foreach(FacilityRecipeListing frl in recipeFacilityComp.Props.facilityLinkRecipes)
                {
                    foreach(Thing thing in facilityComp.LinkedFacilitiesListForReading)
                    {
                        if(thing.def == frl.facility)
                        {
                            if(thing.TryGetComp<CompQuality>() != null && frl.minQuality != QualityCategory.Awful)
                            {
                                thing.TryGetQuality(out QualityCategory qc);
                                if(qc < frl.minQuality)
                                {
                                    continue;
                                }
                            }
                            AddRecipeList(frl.recipes, facilityComp);
                        }
                    }
                }
            }
        }

        public static void AddRecipeList(List<RecipeDef> recipes, CompAffectedByFacilities facilityComp)
        {
            foreach(RecipeDef recipe in recipes)
            {
                if (!facilityComp.parent.def.AllRecipes.Contains(recipe))
                {
                    facilityComp.parent.def.AllRecipes.Add(recipe);
                }
                else if(TabulaRasaMod.settings.verboseLogging)
                {
                    LogUtil.LogWarning($"Recipe {recipe.label} with defName {recipe.defName} already exists on {facilityComp.parent.def.label} with defName {facilityComp.parent.def.defName}. Probably intended behaviour, skipping.");
                }
            }
        }
    }
}
