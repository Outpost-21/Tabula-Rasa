using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace HRF
{
	[HarmonyPatch(typeof(BillUtility), "MakeNewBill")]
	public static class Patch_MakeNewBill
	{ 
		public static bool Prefix(ref Bill __result, RecipeDef recipe, Precept_ThingStyle precept = null)
        {
			if (recipe.HasModExtension<RecipeResourceIngredients>())
            {
				if (recipe.UsesUnfinishedThing)
				{
					__result = new Bill_ResourceWithUft(recipe, precept);
				}
				else
                {
					__result = new Bill_Resource(recipe, precept);
				}
				return false;
			}
			return true;
        }
	}

    [HarmonyPatch(typeof(HealthCardUtility), "CreateSurgeryBill")]
    public static class Patch_CreateSurgeryBill
    {
        public static bool Prefix(Pawn medPawn, RecipeDef recipe, BodyPartRecord part)
        {
            if (recipe.HasModExtension<RecipeResourceIngredients>())
            {
                CreateSurgeryBill(medPawn, recipe, part);
                return false;
            }
            return true;
        }
        private static void CreateSurgeryBill(Pawn medPawn, RecipeDef recipe, BodyPartRecord part)
        {
            Bill_ResourceMedical bill_Medical = new Bill_ResourceMedical(recipe, null);
            medPawn.BillStack.AddBill(bill_Medical);
            bill_Medical.Part = part;
            if (recipe.conceptLearned != null)
            {
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
            }
            Map map = medPawn.Map;
            if (!map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
            {
                Bill.CreateNoPawnsWithSkillDialog(recipe);
            }
            if (!medPawn.InBed() && medPawn.RaceProps.IsFlesh)
            {
                if (medPawn.RaceProps.Humanlike)
                {
                    if (!map.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed && RestUtility.CanUseBedEver(medPawn, x.def) && ((Building_Bed)x).Medical))
                    {
                        Messages.Message("MessageNoMedicalBeds".Translate(), medPawn, MessageTypeDefOf.CautionInput, historical: false);
                    }
                }
                else if (!map.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed && RestUtility.CanUseBedEver(medPawn, x.def)))
                {
                    Messages.Message("MessageNoAnimalBeds".Translate(), medPawn, MessageTypeDefOf.CautionInput, historical: false);
                }
            }
            if (medPawn.Faction != null && !medPawn.Faction.Hidden && !medPawn.Faction.HostileTo(Faction.OfPlayerSilentFail) && recipe.Worker.IsViolationOnPawn(medPawn, part, Faction.OfPlayerSilentFail))
            {
                Messages.Message("MessageMedicalOperationWillAngerFaction".Translate(medPawn.HomeFaction), medPawn, MessageTypeDefOf.CautionInput, historical: false);
            }
            ThingDef minRequiredMedicine = GetMinRequiredMedicine(recipe);
            if (minRequiredMedicine != null && medPawn.playerSettings != null && !medPawn.playerSettings.medCare.AllowsMedicine(minRequiredMedicine))
            {
                Messages.Message("MessageTooLowMedCare".Translate(minRequiredMedicine.label, medPawn.LabelShort, medPawn.playerSettings.medCare.GetLabel(), medPawn.Named("PAWN")), medPawn, MessageTypeDefOf.CautionInput, historical: false);
            }
            recipe.Worker.CheckForWarnings(medPawn);
        }

        private static List<ThingDef> tmpMedicineBestToWorst = new List<ThingDef>();
        private static ThingDef GetMinRequiredMedicine(RecipeDef recipe)
        {
            tmpMedicineBestToWorst.Clear();
            List<ThingDef> allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                if (allDefsListForReading[i].IsMedicine)
                {
                    tmpMedicineBestToWorst.Add(allDefsListForReading[i]);
                }
            }
            tmpMedicineBestToWorst.SortByDescending((ThingDef x) => x.GetStatValueAbstract(StatDefOf.MedicalPotency));
            ThingDef thingDef = null;
            for (int j = 0; j < recipe.ingredients.Count; j++)
            {
                ThingDef thingDef2 = null;
                for (int k = 0; k < tmpMedicineBestToWorst.Count; k++)
                {
                    if (recipe.ingredients[j].filter.Allows(tmpMedicineBestToWorst[k]))
                    {
                        thingDef2 = tmpMedicineBestToWorst[k];
                    }
                }
                if (thingDef2 != null && (thingDef == null || thingDef2.GetStatValueAbstract(StatDefOf.MedicalPotency) > thingDef.GetStatValueAbstract(StatDefOf.MedicalPotency)))
                {
                    thingDef = thingDef2;
                }
            }
            tmpMedicineBestToWorst.Clear();
            return thingDef;
        }
    }

    [HarmonyPatch(typeof(Dialog_BillConfig), "DoWindowContents")]
	public static class Patch_DoWindowContents
	{
		[HarmonyPriority(int.MaxValue)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			var codes = instructions.ToList();

			for (var i = 0; i < codes.Count; i++)
            {
				if (i > 2 && codes[i - 1].opcode == OpCodes.Blt_S && codes[i - 2].Calls(AccessTools.Method(typeof(List<IngredientCount>), "get_Count")) 
					&& codes[i].opcode == OpCodes.Ldloc_S && codes[i].operand is LocalBuilder lb && lb.LocalIndex >= 34)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_S, lb.LocalIndex);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_BillConfig), "bill"));
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Bill), "recipe"));
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_DoWindowContents), "AppendLine"));
                }
				yield return codes[i];

            }
		}

		public static void AppendLine(StringBuilder stringBuilder, RecipeDef recipe)
        {
			var extension = recipe.GetModExtension<RecipeResourceIngredients>();
			if (extension != null)
            {
				foreach (var resourceCost in extension.recourseCostList)
                {
					if (resourceCost.cost > 0)
                    {
						stringBuilder.AppendLine("BillRequires".Translate(resourceCost.cost, resourceCost.resource.label));
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Toils_Recipe), nameof(Toils_Recipe.DoRecipeWork))]
	public static class Toils_RecipePatch
    {
		public static void Postfix(Toil __result)
        {
			__result.AddPreTickAction(delegate
			{
				Pawn actor = __result.actor;
				Job curJob = actor.jobs.curJob;
				if (curJob.bill is Bill_Resource bill_resource)
                {
					DoWork(bill_resource.recipe, actor, bill_resource.Extension, bill_resource.consumedResources);
				}
				else if (curJob.bill is Bill_ResourceWithUft bill_ResourceWithUft)
                {
					DoWork(bill_ResourceWithUft.recipe, actor, bill_ResourceWithUft.Extension, bill_ResourceWithUft.consumedResources);
				}
			});
        }

        public static void DoWork(RecipeDef recipe, Pawn p, RecipeResourceIngredients extension, Dictionary<HediffResourceDef, float> consumedResources)
        {
            if (p.jobs.curDriver is JobDriver_DoBill jobDriver_DoBill)
            {
                if (jobDriver_DoBill.BillGiver is Thing thing && !p.CanUseIt(thing, out _))
                {
                    HRFLog.Message("Ending job: " + p.CurJob + " - cannot use it");
                    p.jobs.EndCurrentJob(JobCondition.Incompletable);
                    return;
                }
                UnfinishedThing uft = p.CurJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                float num = ((p.CurJob.RecipeDef.workSpeedStat == null) ? 1f : p.GetStatValue(p.CurJob.RecipeDef.workSpeedStat));
                if (p.CurJob.RecipeDef.workTableSpeedStat != null)
                {
                    Building_WorkTable building_WorkTable = jobDriver_DoBill.BillGiver as Building_WorkTable;
                    if (building_WorkTable != null)
                    {
                        num *= building_WorkTable.GetStatValue(p.CurJob.RecipeDef.workTableSpeedStat);
                    }
                }

                if (DebugSettings.fastCrafting)
                {
                    num *= 30f;
                }
                var workTotalAmount = recipe.WorkAmountTotal(uft?.Stuff);
                foreach (var resourceCost in extension.recourseCostList)
                {
                    if (consumedResources is null)
                    {
                        consumedResources = new Dictionary<HediffResourceDef, float>();
                    }
                    if (!consumedResources.ContainsKey(resourceCost.resource))
                    {
                        consumedResources[resourceCost.resource] = 0;
                    }

                    var diff = resourceCost.cost - consumedResources[resourceCost.resource];
                    var curCost = resourceCost.cost / (workTotalAmount / num);
                    if (diff != 0)
                    {
                        var hediff = p.health.hediffSet.GetFirstHediffOfDef(resourceCost.resource) as HediffResource;
                        if (hediff is null || diff > 0 && (int)hediff.ResourceAmount < (int)diff)
                        {
                            HRFLog.Message("Ending job: " + p.CurJob + " - hediff.ResourceAmount: " + hediff.ResourceAmount + " - diff: " + diff);
                            p.jobs.EndCurrentJob(JobCondition.Incompletable);
                            return;
                        }
                        else
                        {
                            if (resourceCost.cost < 0 && hediff.ResourceAmount >= hediff.ResourceCapacity)
                            {
                                continue;
                            }
                            var toConsume = diff > 0 ? diff >= curCost ? curCost : diff : diff < curCost ? curCost : diff;
                            hediff.ChangeResourceAmount(-toConsume);
                            if (consumedResources.ContainsKey(resourceCost.resource))
                            {
                                consumedResources[resourceCost.resource] += toConsume;
                            }
                            else
                            {
                                consumedResources[resourceCost.resource] = toConsume;
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    public static class GenRecipe_MakeRecipeProducts_Patch
    {
        public class Data
        {
            public List<ThingDefCountClass> oldProducts;
            public RecipeOutcome pickedRecipeOutcome;
        }
        public static void Prefix(out Data __state, RecipeDef recipeDef, Pawn worker)
        {
            var recipeOutcome = GetRecipeOutcome(recipeDef, worker);
            if (recipeOutcome != null)
            {
                __state = new Data
                {
                    oldProducts = recipeDef.products.ListFullCopy(),
                    pickedRecipeOutcome = recipeOutcome
                };
                recipeDef.products = recipeOutcome.products;
            }
            else
            {
                __state = null;
            }
        }

        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Data __state, RecipeDef recipeDef, Pawn worker)
        {
            foreach (var r in __result)
            {
                if (__state != null)
                {
                    if (!__state.pickedRecipeOutcome.topLeftMessageSuccessKey.NullOrEmpty())
                    {
                        Messages.Message(__state.pickedRecipeOutcome.topLeftMessageSuccessKey.Translate(worker.Named("WORKER"), r.LabelCap), r, MessageTypeDefOf.PositiveEvent);
                    }
                    if (!__state.pickedRecipeOutcome.letterTitleSuccessKey.NullOrEmpty())
                    {
                        Find.LetterStack.ReceiveLetter(__state.pickedRecipeOutcome.letterTitleSuccessKey.Translate(worker.Named("WORKER"), r.LabelCap),
                            __state.pickedRecipeOutcome.letterDescriptionSuccessKey.Translate(worker.Named("WORKER"), r.LabelCap), LetterDefOf.PositiveEvent, r);
                    }
                    if (__state.pickedRecipeOutcome.soundDef != null)
                    {
                        __state.pickedRecipeOutcome.soundDef.PlayOneShot(new TargetInfo(worker.Position, worker.Map));
                    }
                }
                yield return r;
            }
            if (__state != null)
            {
                recipeDef.products = __state.oldProducts;
            }
        }

        private static RecipeOutcome GetRecipeOutcome(RecipeDef recipeDef, Pawn worker)
        {
            var extension = recipeDef.GetModExtension<RecipeOutcomes>();
            if (extension != null)
            {
                foreach (var outcome in extension.recipeOutcomes)
                {
                    if (outcome.costs.All(x => worker.HasResource(x.resource, x.cost)))
                    {
                        return outcome;
                    }
                }
            }
            return null;
        }
    }
}
