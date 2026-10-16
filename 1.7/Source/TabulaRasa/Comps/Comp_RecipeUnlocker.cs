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

        public List<RecipeDef> originalRecipeDefs = new List<RecipeDef>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            originalRecipeDefs = parent.def.AllRecipes;
        }

        public static void UpdateRecipes(CompAffectedByFacilities facilityComp)
        {
            List<RecipeDef> resultingRecipes = new List<RecipeDef>();
            if (facilityComp.parent.TryGetComp<Comp_RecipesFromFacilities>() != null)
            {
                Comp_RecipesFromFacilities recipeFacilityComp = facilityComp.parent.TryGetComp<Comp_RecipesFromFacilities>();

                foreach (FacilityRecipeListing frl in recipeFacilityComp.Props.facilityLinkRecipes)
                {
                    bool anyMatchingFacilities = false;
                    foreach (Thing thing in facilityComp.LinkedFacilitiesListForReading)
                    {
                        if (thing.def == frl.facility)
                        {
                            if (thing.TryGetComp<CompQuality>() != null && frl.minQuality != QualityCategory.Awful)
                            {
                                thing.TryGetQuality(out QualityCategory qc);
                                if (qc < frl.minQuality)
                                {
                                    continue;
                                }
                            }
                            anyMatchingFacilities = true;
                        }
                    }
                    if (anyMatchingFacilities)
                    {
                        foreach(RecipeDef recipe in frl.recipes)
                        {
                            if (!resultingRecipes.Contains(recipe))
                            {
                                resultingRecipes.Add(recipe);
                            }
                        }
                    }
                }
                facilityComp.parent.def.AllRecipes.Clear();
                facilityComp.parent.def.AllRecipes.AddRange(recipeFacilityComp.originalRecipeDefs);
                foreach(RecipeDef recipe in resultingRecipes)
                {
                    if (!facilityComp.parent.def.AllRecipes.Contains(recipe))
                    {
                        facilityComp.parent.def.AllRecipes.Add(recipe);
                    }
                }
            }

        }
    }
}
