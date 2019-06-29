using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using AlienRace;

namespace O21Toolbox.PawnConverter
{
    public class Util_PawnConvert
    {
        public static bool IsRequiredSex(Pawn pawn, PawnConvertingDef recipe)
        {
            if (recipe.requiredSex != null)
            {
                if (recipe.requiredSex != pawn.gender.ToString())
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsViableRace(Pawn pawn, PawnConvertingDef recipe)
        {
            if(recipe.inputDefs != null)
            {
                if (!recipe.inputDefs.Contains(pawn.def))
                {
                    return false;
                }
            }
            return true;
        }

        #region PawnConversion_AlienToAlien
        public static Pawn PawnConversion(Pawn pawnToConvert, PawnConvertingDef recipe)
        {

            PawnGenerationRequest request = new PawnGenerationRequest(
                recipe.outputDef,
                faction: Faction.OfPlayer,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false,
                colonistRelationChanceFactor: 0f,
                fixedBiologicalAge: pawnToConvert.ageTracker.AgeBiologicalYearsFloat,
                fixedChronologicalAge: pawnToConvert.ageTracker.AgeChronologicalYearsFloat,
                allowFood: false);
            Pawn newPawn = PawnGenerator.GeneratePawn(request);
            Pawn pawn = PawnGenerator.GeneratePawn(request);


            // Transfer everything from old pawn to new pawn
            pawn.drugs = pawnToConvert.drugs;
            pawn.foodRestriction = pawnToConvert.foodRestriction;
            // pawn.guilt = pawnToConvert.guilt; - Caused issues with thoughts. Didn't seem necessary.
            // pawn.health = pawnToConvert.health; - Caused issues with taking damage, to the point of making pawns invulnerable. Didn't seem necessary.
            pawn.health.hediffSet = pawnToConvert.health.hediffSet;
            // pawn.needs = pawnToConvert.needs; - Caused issues with needs. Didn't seem necessary.
            pawn.records = pawnToConvert.records;
            pawn.skills = pawnToConvert.skills;

            TransferStory(pawn, pawnToConvert, recipe);

            pawn.timetable = pawnToConvert.timetable;
            pawn.workSettings = pawnToConvert.workSettings;
            pawn.Name = pawnToConvert.Name;

            if (recipe.outputSex != null)
            {
                Gender outputGender = GetOutputGender(pawnToConvert, recipe);
            }
            else
            {
                if (pawnToConvert.def.race.hasGenders)
                {
                    pawn.gender = pawnToConvert.gender;
                }
            }

            ApplyHairChange(pawn, newPawn, recipe);

            ApplyHairColor(pawn, newPawn, recipe);

            ApplySkinChange(pawn, newPawn, recipe);

            ApplyHeadChange(pawn, newPawn, recipe);

            ApplyBodyChange(pawn, recipe);

            RemoveRequiredHediffs(pawn, recipe);

            ApplyForcedHediff(pawn, recipe);

            FixPawnRelations(pawn, pawnToConvert);

            FixPawnEquipment(pawn, pawnToConvert, recipe);

            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return pawn;
        }

        private static Pawn FixPawnEquipment(Pawn pawn, Pawn pawnToConvert, PawnConvertingDef recipe)
        {
            // No pregenerated equipment.
            pawn?.equipment?.DestroyAllEquipment();
            pawn?.apparel?.DestroyAll();
            pawn?.inventory?.DestroyAll();

            if (recipe.dropEverything)
            {
                pawnToConvert.equipment.DropAllEquipment(pawnToConvert.Position);
                pawnToConvert.apparel.DropAll(pawnToConvert.Position);
                pawnToConvert.inventory.DropAllNearPawn(pawnToConvert.Position);
            }
            else
            {
                // Transfer Old Equipmnet.
                pawn.equipment = pawnToConvert.equipment;
                pawn.apparel = pawnToConvert.apparel;
                pawn.inventory = pawnToConvert.inventory;
            }

            return pawn;
        }

        public static Pawn TransferStory(Pawn newPawn, Pawn oldPawn, PawnConvertingDef recipe)
        {
            if (oldPawn.TryGetComp<AlienPartGenerator.AlienComp>() != null && newPawn.TryGetComp<AlienPartGenerator.AlienComp>() != null)
            {
                newPawn.story.childhood = oldPawn.story.childhood;
                newPawn.story.adulthood = oldPawn.story.adulthood;
                newPawn.story.title = oldPawn.story.title;
                newPawn.story.traits = oldPawn.story.traits;
                if (recipe.forcedHead == null)
                {
                    newPawn.story.crownType = oldPawn.story.crownType;
                }
                if (!recipe.forcedSkinColor && !recipe.randomSkinColor)
                {
                    newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor = oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor;
                    newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond = oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond;
                }
                if (!recipe.randomHair && recipe.forcedHair == null)
                {
                    newPawn.story.hairDef = oldPawn.story.hairDef;
                }
                if (!recipe.randomHairColor && !recipe.forcedHairColor)
                {
                    newPawn.story.hairColor = oldPawn.story.hairColor;
                    if (oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond != null)
                    {
                        newPawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond = oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond;
                    }
                }
                newPawn.Drawer.renderer.graphics.ResolveAllGraphics();
            }

            return newPawn;
        }

        public static Pawn ApplyHairChange(Pawn pawn, Pawn newPawn, PawnConvertingDef recipe)
        {
            // Change hair if needed.
            if (!recipe.randomHair)
            {
                if (recipe.forcedHair != null)
                {
                    pawn.story.hairDef = recipe.forcedHair;
                }
            }

            if (recipe.randomHair)
            {
                pawn.story.hairDef = newPawn.story.hairDef;
            }

            return pawn;
        }

        public static Pawn ApplyHairColor(Pawn pawn, Pawn newPawn, PawnConvertingDef recipe)
        {
            // Change hair colour if needed.
            if (recipe.forcedHairColor)
            {
                if (recipe.forcedHairColorOne)
                {
                    pawn.story.hairColor = recipe.hairColorOne;
                }
                if (recipe.forcedHairColorTwo)
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond = recipe.hairColorTwo;
                }
            }
            if (recipe.randomHairColor)
            {
                pawn.story.hairColor = newPawn.story.hairColor;
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond;
            }
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return pawn;
        }

        public static Pawn ApplySkinChange(Pawn pawn, Pawn newPawn, PawnConvertingDef recipe)
        {
            // Change skin colour if needed.
            if (recipe.forcedSkinColor)
            {
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor = recipe.forcedSkinColorOne;
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond = recipe.forcedSkinColorTwo;
            }
            if (recipe.randomSkinColor)
            {
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor;
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond;
            }
            else
            {

            }
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return pawn;
        }

        public static Pawn ApplyHeadChange(Pawn pawn, Pawn newPawn, PawnConvertingDef recipe)
        {
            // Change crown if needed.
            if (recipe.forcedHead != null)
            {
                if (recipe.forcedHead == "RANDOM")
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType;
                    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                }
                else
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType = recipe.forcedHead;
                    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                }
            }

            return pawn;
        }

        public static Pawn ApplyBodyChange(Pawn pawn, PawnConvertingDef recipe)
        {
            // Change body if needed.
            if (recipe.forcedBody != null)
            {
                pawn.story.bodyType = recipe.forcedBody;
                pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            }

            return pawn;
        }

        public static Pawn RemoveRequiredHediffs(Pawn pawn, PawnConvertingDef recipe)
        {
            // Remove required hediffs if needed.
            if (recipe.removeRequiredHediffs)
            {
                IEnumerable<HediffDef> enumerable = from def in recipe.requiredHediffs
                                                    where pawn.health.hediffSet.HasHediff(def, false)
                                                    select def;
                foreach (HediffDef current in enumerable)
                {
                    pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(current));
                }
            }

            return pawn;
        }

        public static Pawn ApplyForcedHediff(Pawn pawn, PawnConvertingDef recipe)
        {
            // Apply Forced Hediff if needed.
            if (recipe.forcedHediff != null)
            {
                if (!pawn.health.hediffSet.hediffs.Contains(recipe.forcedHediff))
                {
                    pawn.health.hediffSet.AddDirect(recipe.forcedHediff);
                }
                Log.Message("Pawn already has forced Hediff, new hediff was not applied.", false);
            }

            return pawn;
        }

        public static Gender GetOutputGender(Pawn pawn, PawnConvertingDef recipe)
        {
            Gender outputGender;
            if (recipe.outputSex != null)
            {
                string outputSex = recipe.outputSex;
                switch (outputSex)
                {
                    case "Male":
                        outputGender = Gender.Male;
                        break;
                    case "Female":
                        outputGender = Gender.Male;
                        break;
                    default:
                        Log.Message("Defined sex/gender does not exist in this context. Defaulting to original.", false);
                        outputGender = pawn.gender;
                        break;
                }
            }
            else
            {
                outputGender = pawn.gender;
            };
            return outputGender;
        }
        #endregion

        #region PawnConversion_HumanToAlien
        public static Pawn HumanPawnConversion(Pawn pawnToConvert, PawnConvertingDef recipe)
        {
            Gender outputGender = GetOutputGender(pawnToConvert, recipe);

            PawnGenerationRequest request = new PawnGenerationRequest(
                recipe.outputDef,
                faction: Faction.OfPlayer,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false,
                colonistRelationChanceFactor: 0f,
                fixedBiologicalAge: pawnToConvert.ageTracker.AgeBiologicalYearsFloat,
                fixedChronologicalAge: pawnToConvert.ageTracker.AgeChronologicalYearsFloat,
                fixedGender: outputGender,
                allowFood: false);
            Pawn newPawn = PawnGenerator.GeneratePawn(request);
            Pawn pawn = PawnGenerator.GeneratePawn(request);

            // No pregenerated equipment.
            pawn?.equipment?.DestroyAllEquipment();
            pawn?.apparel?.DestroyAll();
            pawn?.inventory?.DestroyAll();

            // Transfer everything from old pawn to new pawn
            pawn.drugs = pawnToConvert.drugs;
            pawn.foodRestriction = pawnToConvert.foodRestriction;
            // pawn.guilt = pawnToConvert.guilt; - Caused issues with thoughts. Didn't seem necessary.
            // pawn.health = pawnToConvert.health; - Caused issues with taking damage, to the point of making pawns invulnerable. Didn't seem necessary.
            pawn.health.hediffSet = pawnToConvert.health.hediffSet;
            // pawn.needs = pawnToConvert.needs; - Caused issues with needs. Didn't seem necessary.
            pawn.records = pawnToConvert.records;
            pawn.skills = pawnToConvert.skills;

            //TransferStory_Human(pawn, pawnToConvert, recipe);

            pawn.timetable = pawnToConvert.timetable;
            pawn.workSettings = pawnToConvert.workSettings;
            pawn.Name = pawnToConvert.Name;

            ApplyHairChange(pawn, newPawn, recipe);

            ApplyHairColor(pawn, newPawn, recipe);

            ApplySkinChange(pawn, newPawn, recipe);

            ApplyHeadChange(pawn, newPawn, recipe);

            ApplyBodyChange(pawn, recipe);

            RemoveRequiredHediffs(pawn, recipe);

            ApplyForcedHediff(pawn, recipe);

            FixPawnRelations(pawn, pawnToConvert);

            FixPawnEquipment(pawn, pawnToConvert, recipe);

            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return pawn;
        }

        public static Pawn TransferStory_Human(Pawn newPawn, Pawn oldPawn, PawnConvertingDef recipe)
        {
            newPawn.story = oldPawn.story;

            newPawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return newPawn;
        }
        #endregion

        public static Pawn FixPawnRelations(Pawn newPawn, Pawn oldPawn)
        {
            foreach(DirectPawnRelation relation in oldPawn.relations.DirectRelations)
            {
                relation.otherPawn.relations.AddDirectRelation(relation.def, newPawn);
                relation.otherPawn.relations.RemoveDirectRelation(relation.def, oldPawn);
            }

            return newPawn;
        }
    }
}
