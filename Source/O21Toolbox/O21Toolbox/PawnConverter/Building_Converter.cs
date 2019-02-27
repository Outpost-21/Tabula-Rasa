using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using AlienRace;

namespace O21Toolbox.PawnConverter
{
    public class Building_Converter : Building_Casket
    {
        [DefOf]
        public static class ConverterDefOf
        {
            public static JobDef EnterConverter;
        }

        protected int ICookingTicking = 0;

        public int ICookingTime = 12000; //60k = 1 day.

        public CompPowerTrader powerComp;

        public CompGlower glowerComp;

        public Comp_Converter converterComp;

        public ColorInt red = new ColorInt(252, 187, 113, 0);

        public ColorInt green = new ColorInt(100, 255, 100, 0);

        public ColorInt currentColour;

        public bool powerOn => powerComp.PowerOn;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            glowerComp = this.GetComp<CompGlower>();
            powerComp = this.GetComp<CompPowerTrader>();
            converterComp = this.GetComp<Comp_Converter>();
            ICookingTime = converterComp.Props.cookingTime;
            red = converterComp.Props.redLight;
            green = converterComp.Props.greenLight;
            ChangeColour(green);
            currentColour = green;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (converterComp.Props.requiresPower)
            {
                if (!powerComp.PowerOn)
                {
                    FloatMenuOption item = new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    return new List<FloatMenuOption>
                        {
                            item
                        };
                }
            }
            if (!ReservationUtility.CanReserve(myPawn, this, 1))
            {
                FloatMenuOption item2 = new FloatMenuOption(Translator.Translate("CannotUseReserved"), (Action)null, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!ReachabilityUtility.CanReach(myPawn, this, PathEndMode.OnCell, Danger.Some, false))
            {
                FloatMenuOption item3 = new FloatMenuOption(Translator.Translate("CannotUseNoPath"), (Action)null, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (converterComp.Props.requiredSex != null)
            {
                if(converterComp.Props.requiredSex == Gender.Male.ToString() && Gender.Male != myPawn.gender)
                {
                    FloatMenuOption item4 = new FloatMenuOption("CannotUseNotViable".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    return new List<FloatMenuOption>
                    {
                        item4
                    };
                }
                if(converterComp.Props.requiredSex == Gender.Female.ToString() && Gender.Female != myPawn.gender)
                {
                    FloatMenuOption item4 = new FloatMenuOption("CannotUseNotViable".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    return new List<FloatMenuOption>
                    {
                        item4
                    };
                }
            }
            if(converterComp.Props.inputDefs != null)
            {
                if (!converterComp.Props.inputDefs.Contains(myPawn.def.defName))
                {
                    FloatMenuOption item5 = new FloatMenuOption("CannotUseNotViable".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    return new List<FloatMenuOption>
                    {
                        item5
                    };
                }
            }
            if (converterComp.Props.requiredHediffs != null)
            {
                if (!this.HasRequiredHediffs(myPawn))
                {
                    FloatMenuOption item5 = new FloatMenuOption("CannotUseNotViable".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    return new List<FloatMenuOption>
                    {
                        item5
                    };
                }
            }
            if (!this.HasAnyContents)
            {
                FloatMenuOption item6 = new FloatMenuOption(Translator.Translate("EnterConverter"), (Action)delegate
                {
                    Job val2 = new Job(ConverterDefOf.EnterConverter, this);
                    ReservationUtility.Reserve(myPawn, this, val2);
                    myPawn.jobs.TryTakeOrderedJob(val2);
                }, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
            {
                item6
            };
            }
            return null;
        }

        public bool HasRequiredHediffs(Pawn pawn)
        {
            if(converterComp.Props.requiredHediffs.All(x => pawn.health.hediffSet.HasHediff(x, false)))
            {
                if (converterComp.Props.hediffSeverityMatters)
                {
                    IEnumerable<HediffDef> enumerable = from def in converterComp.Props.requiredHediffs
                                                        where pawn.health.hediffSet.HasHediff(def, false)
                                                        select def;
                    foreach (HediffDef current in enumerable)
                    {
                        if (pawn.health.hediffSet.GetFirstHediffOfDef(current, false) != null)
                        {
                            if (pawn.health.hediffSet.GetFirstHediffOfDef(current, false).Severity < converterComp.Props.requiredHediffSeverity)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void CookIt()
        {
            ThingOwner convertedContainer = new ThingOwner<Thing>();
            foreach (Thing item in this.innerContainer)
            {
                if (item is Pawn val)
                {
                    convertedContainer.TryAdd(ConversionProcess(val));
                }
            }
            this.innerContainer.ClearAndDestroyContents();
            this.innerContainer = convertedContainer;
        }

        protected Pawn ConversionProcess(Pawn pawnToConvert)
        {
            Gender outputGender = GetOutputGender(pawnToConvert);

            PawnGenerationRequest request = new PawnGenerationRequest(
                converterComp.Props.outputDef,
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
            // pawn.guilt = pawnToConvert.guilt;
            // pawn.health = pawnToConvert.health;
            pawn.health.hediffSet = pawnToConvert.health.hediffSet;
            // pawn.needs = pawnToConvert.needs;
            pawn.records = pawnToConvert.records;
            pawn.skills = pawnToConvert.skills;

            TransferStory(pawn, pawnToConvert);

            pawn.timetable = pawnToConvert.timetable;
            pawn.workSettings = pawnToConvert.workSettings;
            pawn.Name = pawnToConvert.Name;

            ApplyHairChange(pawn, newPawn);

            ApplyHairColor(pawn, newPawn);

            ApplySkinChange(pawn, newPawn);

            ApplyHeadChange(pawn, newPawn);

            ApplyBodyChange(pawn);

            RemoveRequiredHediffs(pawn);

            ApplyForcedHediff(pawn);

            // FixPawnRelations(pawn, pawnToConvert);

            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            
            return pawn;
        }

        protected Pawn TransferStory(Pawn newPawn, Pawn oldPawn)
        {
            if (converterComp.Props.useOldStoryTransfer)
            {
                newPawn.story = oldPawn.story;

                newPawn.Drawer.renderer.graphics.ResolveAllGraphics();
            }
            else
            {
                newPawn.story.childhood = oldPawn.story.childhood;
                newPawn.story.adulthood = oldPawn.story.adulthood;
                newPawn.story.title = oldPawn.story.title;
                newPawn.story.traits = oldPawn.story.traits;
                if (converterComp.Props.forcedHead == null)
                {
                    newPawn.story.crownType = oldPawn.story.crownType;
                }
                if (!converterComp.Props.forcedSkinColor && !converterComp.Props.randomSkinColor)
                {
                    newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor = oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor;
                    newPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond = oldPawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond;
                }
                if (!converterComp.Props.randomHair && converterComp.Props.forcedHair == null)
                {
                    newPawn.story.hairDef = oldPawn.story.hairDef;
                }
                if (!converterComp.Props.randomHairColor && !converterComp.Props.forcedHairColor)
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

        protected Pawn ApplyHairChange(Pawn pawn, Pawn newPawn)
        {
            // Change hair if needed.
            if (!converterComp.Props.randomHair)
            {
                if (converterComp.Props.forcedHair != null)
                {
                    pawn.story.hairDef = converterComp.Props.forcedHair;
                }
            }

            if (converterComp.Props.randomHair)
            {
                pawn.story.hairDef = newPawn.story.hairDef;
            }

            return pawn;
        }

        protected Pawn ApplyHairColor(Pawn pawn, Pawn newPawn)
        {
            // Change hair colour if needed.
            if (converterComp.Props.forcedHairColor)
            {
                if (converterComp.Props.forcedHairColorOne)
                {
                    pawn.story.hairColor = converterComp.Props.hairColorOne;
                }
                if (converterComp.Props.forcedHairColorTwo)
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond = converterComp.Props.hairColorTwo;
                }
            }
            if (converterComp.Props.randomHairColor)
            {
                pawn.story.hairColor = newPawn.story.hairColor;
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().hairColorSecond;
            }
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            return pawn;
        }

        protected Pawn ApplySkinChange(Pawn pawn, Pawn newPawn)
        {
            // Change skin colour if needed.
            if (converterComp.Props.forcedSkinColor)
            {
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColor = converterComp.Props.forcedSkinColorOne;
                pawn.TryGetComp<AlienPartGenerator.AlienComp>().skinColorSecond = converterComp.Props.forcedSkinColorTwo;
            }
            if (converterComp.Props.randomSkinColor)
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

        protected Pawn ApplyHeadChange(Pawn pawn, Pawn newPawn)
        {
            // Change crown if needed.
            if (converterComp.Props.forcedHead != null)
            {
                if (converterComp.Props.forcedHead == "RANDOM")
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType = newPawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType;
                    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                }
                else
                {
                    pawn.TryGetComp<AlienPartGenerator.AlienComp>().crownType = converterComp.Props.forcedHead;
                    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                }
            }

            return pawn;
        }

        protected Pawn ApplyBodyChange(Pawn pawn)
        {
            // Change body if needed.
            if (converterComp.Props.forcedBody != null)
            {
                pawn.story.bodyType = converterComp.Props.forcedBody;
                pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            }

            return pawn;
        }

        protected Pawn RemoveRequiredHediffs(Pawn pawn)
        {
            // Remove required hediffs if needed.
            if (converterComp.Props.removeRequiredHediffs)
            {
                IEnumerable<HediffDef> enumerable = from def in converterComp.Props.requiredHediffs
                                                    where pawn.health.hediffSet.HasHediff(def, false)
                                                    select def;
                foreach (HediffDef current in enumerable)
                {
                    pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(current));
                }
            }

            return pawn;
        }

        protected Pawn ApplyForcedHediff(Pawn pawn)
        {
            // Apply Forced Hediff if needed.
            if (converterComp.Props.forcedHediff != null)
            {
                if (!pawn.health.hediffSet.hediffs.Contains(converterComp.Props.forcedHediff))
                {
                    pawn.health.hediffSet.AddDirect(converterComp.Props.forcedHediff);
                }
                Log.Message("Pawn already has forced Hediff, new hediff was not applied.", false);
            }

            return pawn;
        }

        private Gender GetOutputGender(Pawn inputPawn)
        {
            Gender outputGender;
            if (converterComp.Props.outputSex != null)
            {
                string outputSex = converterComp.Props.outputSex;
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
                        outputGender = inputPawn.gender;
                        break;
                }
            }
            else
            {
                outputGender = inputPawn.gender;
            };
            return outputGender;
        }

        public override void Tick()
        {
            if (converterComp.Props.requiresPower)
            {
                if (powerComp.PowerOn)
                {
                    if (this.HasAnyContents)
                    {
                        //Not the best way but this just ticks up until max is reached.
                        //It would be better to check the "endtime" with the Tickfinder
                        ICookingTicking++;
                        if (currentColour != red)
                        {
                            ChangeColour(red);
                        }
                        if (ICookingTicking >= ICookingTime)
                        {
                            CookIt();
                            this.EjectContents();
                            ICookingTicking = 0;
                        }
                    }
                    else if (currentColour != green)
                    {
                        ChangeColour(green);
                    }
                }
                else
                {
                    if (currentColour != red)
                    {
                        ChangeColour(red);
                    }
                    ICookingTicking = 0;
                    if (this.HasAnyContents)
                    {
                        this.EjectContents();
                    }
                }
            }
            if (!converterComp.Props.requiresPower)
            {
                if (this.HasAnyContents)
                {
                    //Not the best way but this just ticks up until max is reached.
                    //It would be better to check the "endtime" with the Tickfinder
                    ICookingTicking++;
                    if (ICookingTicking >= ICookingTime)
                    {
                        CookIt();
                        this.EjectContents();
                        ICookingTicking = 0;
                    }
                }
            }
        }

        public override void EjectContents()
        {
            if (!this.Destroyed)
            {
                SoundStarter.PlayOneShot(converterComp.Props.finishingSound, SoundInfo.OnCamera());
            }
            ICookingTicking = 0;
            this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near);
            this.contentsKnown = true;
        }

        public override void Draw()
        {
            base.Draw();
            if (converterComp.Props.timerBarEnabled)
            {
                DrawTimerBar();
            }
        }

        public void DrawTimerBar()
        {
            //Replaced Drawhelper with vanilla drawer here
            GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
            fillableBarRequest.preRotationOffset = converterComp.Props.timerBarOffset;
            fillableBarRequest.size = converterComp.Props.timerBarSize;
            fillableBarRequest.fillPercent = (float)ICookingTicking / (float)ICookingTime;
            fillableBarRequest.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(converterComp.Props.timerBarFill);
            fillableBarRequest.unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(converterComp.Props.timerBarUnfill);
            Rot4 rotation = this.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            fillableBarRequest.rotation = rotation;
            if (fillableBarRequest.rotation == Rot4.West)
            {
                fillableBarRequest.rotation = Rot4.West;
                fillableBarRequest.center = this.DrawPos + Vector3.up * 0.1f + Vector3.back * 0.45f;
            }
            if (fillableBarRequest.rotation == Rot4.North)
            {
                fillableBarRequest.rotation = Rot4.North;
                fillableBarRequest.center = this.DrawPos + Vector3.up * 0.1f + Vector3.left * 0.45f;
            }
            if (fillableBarRequest.rotation == Rot4.East)
            {
                fillableBarRequest.rotation = Rot4.East;
                fillableBarRequest.center = this.DrawPos + Vector3.up * 0.1f + Vector3.forward * 0.45f;
            }
            if (fillableBarRequest.rotation == Rot4.South)
            {
                fillableBarRequest.rotation = Rot4.South;
                fillableBarRequest.center = this.DrawPos + Vector3.up * 0.1f + Vector3.right * 0.45f;
            }
            GenDraw.DrawFillableBar(fillableBarRequest);
        }

        public void ChangeColour(ColorInt colour)
        {

            currentColour = colour;

            if (glowerComp != null)
            {
                float newGlowRadius = glowerComp.Props.glowRadius;
                this.Map.glowGrid.DeRegisterGlower(glowerComp);
                glowerComp.Initialize(new CompProperties_Glower
                {
                    compClass = typeof(CompGlower),
                    glowColor = colour,
                    glowRadius = newGlowRadius
                });
                this.Map.mapDrawer.MapMeshDirty(this.Position, MapMeshFlag.Things);
                this.Map.glowGrid.RegisterGlower(glowerComp);
            }
        }

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDef.Named("CryptosleepCasketAccept").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                return true;
            }
            return false;
        }

        public static Building_Converter FindCryptosleepCasketFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where typeof(Building_Converter).IsAssignableFrom(def.thingClass)
                                               select def;
            foreach (ThingDef current in enumerable)
            {
                Building_Converter building_converter = (Building_Converter)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(current), PathEndMode.InteractionCell, TraverseParms.For(traveler, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, delegate (Thing x)
                {
                    bool arg_33_0;
                    if (!((Building_Converter)x).HasAnyContents)
                    {
                        Pawn traveler2 = traveler;
                        LocalTargetInfo target = x;
                        bool ignoreOtherReservations2 = ignoreOtherReservations;
                        arg_33_0 = traveler2.CanReserve(target, 1, -1, null, ignoreOtherReservations2);
                    }
                    else
                    {
                        arg_33_0 = false;
                    }
                    return arg_33_0;
                }, null, 0, -1, false, RegionType.Set_Passable, false);
                if (building_converter != null)
                {
                    return building_converter;
                }
            }
            return null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ICookingTicking, "ICookingTicking", 0, false);
            Scribe_Values.Look<int>(ref this.ICookingTime, "ICookingTime", 12000, false);
        }
    }

    public class JobDriver_EnterConverter : JobDriver_EnterCryptosleepCasket
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil prepare = Toils_General.Wait(500, TargetIndex.None);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return prepare;
            Toil enter = new Toil();
            enter.initAction = delegate
            {
                Pawn actor = enter.actor;
                Building_Converter pod = (Building_Converter)actor.CurJob.targetA.Thing;
                void action()
                {
                    actor.equipment.DropAllEquipment(actor.Position);
                    actor.apparel.DropAll(actor.Position);
                    actor.inventory.DropAllNearPawn(actor.Position);
                    actor.DeSpawn();
                    pod.TryAcceptThing(actor, true);
                }
                if (!pod.def.building.isPlayerEjectable)
                {
                    int freeColonistsSpawnedOrInPlayerEjectablePodsCount = this.Map.mapPawns.FreeColonistsSpawnedOrInPlayerEjectablePodsCount;
                    if (freeColonistsSpawnedOrInPlayerEjectablePodsCount <= 1)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("CasketWarning".Translate().AdjustedFor(actor), action, false, null));
                    }
                    else
                    {
                        action();
                    }
                }
                else
                {
                    action();
                }
            };
            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;
        }
    }
}
