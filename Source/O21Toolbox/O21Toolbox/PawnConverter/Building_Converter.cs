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
    public class Building_Converter : Building_Casket, IThingHolder
    {
        [DefOf]
        public static class ConverterDefOf
        {
            /// <summary>
            /// Job def telling pawns to enter the converter.
            /// </summary>
            public static JobDef EnterConverter;
        }

        /// <summary>
        /// Converter Component.
        /// </summary>
        protected ConverterProperties converterProperties;

        /// <summary>
        /// Power component.
        /// </summary>
        protected CompPowerTrader powerComp;

        /// <summary>
        /// Flickable component.
        /// </summary>
        protected CompFlickable flickableComp;

        /// <summary>
        /// Current tick progress
        /// </summary>
        protected int ICookingTicking;

        /// <summary>
        /// Time it takes to cook
        /// </summary>
        protected int ICookingTime;

        /// <summary>
        /// Ticks left until pawn is finished converting.
        /// </summary>
        public int conversionTicksLeft = 0;
        /// <summary>
        /// Ticks left until next resource drain tick.
        /// </summary>
        public int nextResourceTick = 0;
        /// <summary>
        /// Set by recipe.
        /// </summary>
        public int conversionTime = 0;

        /// <summary>
        /// Stored ingredients for using while converting.
        /// </summary>
        public ThingOwner<Thing> ingredients = new ThingOwner<Thing>();

        /// <summary>
        /// Recipe chosen for converting pawn.
        /// </summary>
        public PawnConvertingDef chosenRecipe = null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            flickableComp = GetComp<CompFlickable>();
            converterProperties = def.GetModExtension<ConverterProperties>();
        }

        public override void PostMake()
        {
            base.PostMake();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref conversionTicksLeft, "conversionTicksLeft");
            Scribe_Values.Look(ref nextResourceTick, "nextResourceTick");
            Scribe_Values.Look<int>(ref conversionTime, "conversionTime");
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            //Drop ingredients.
            if (mode != DestroyMode.Vanish)
                ingredients.TryDropAll(PositionHeld, MapHeld, ThingPlaceMode.Near);

            base.Destroy(mode);
        }

        /// <summary>
        /// How finished the crafting is in percentage based time. 0.0f to 1.0f
        /// </summary>
        public float ConversionFinishedPercentage
        {
            get
            {
                if (converterProperties.customConversionTime)
                {
                    return ((float)((float)conversionTime - conversionTicksLeft) / (float)conversionTime);
                }
                else
                {
                    return ((float)((float)converterProperties.ticksToConvert - conversionTicksLeft) / (float)converterProperties.ticksToConvert);
                }
            }
        }

        /// <summary>
        /// How many ticks it take to craft a pawn.
        /// </summary>
        public int ConvertingTicks
        {
            get
            {
                if (converterProperties.customConversionTime)
                {
                    return conversionTime;
                }
                else
                {
                    return converterProperties.ticksToConvert;
                }
            }
        }

        /// <summary>
        /// Sets the Storage tab to be visible.
        /// </summary>
        public bool StorageTabVisible => true;

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (converterProperties.requiresPower)
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
            if (!this.HasAnyContents)
            {
                List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();
                foreach (PawnConvertingDef def in DefDatabase<PawnConvertingDef>.AllDefs.OrderBy(def => def.orderID))
                {
                    if (def.recipeUsers != null && def.recipeUsers.Any(x => x == this.def.defName))
                    {
                        bool disabled = false;
                        int reason = 0;
                        string labelText = "";
                        // Check research
                        if (def.requiredResearch != null && !def.requiredResearch.IsFinished)
                        {
                            disabled = true;
                            reason = 0;
                        }
                        // Check input Race
                        if (!Util_PawnConvert.IsViableRace(myPawn, def))
                        {
                            disabled = true;
                            reason = 1;
                        }
                        // Check input Sex
                        if(!Util_PawnConvert.IsRequiredSex(myPawn, def))
                        {
                            disabled = true;
                            reason = 2;
                        }
                        // Check input hediffs
                        if(!HasRequiredHediffs(myPawn, def))
                        {
                            disabled = true;
                            reason = 3;
                        }

                        // If disabled, say why.
                        if (disabled)
                        {
                            switch (reason)
                            {
                                case 0:
                                    labelText = "PawnConverterNeedsResearch".Translate(def.label, def.requiredResearch.LabelCap);
                                    break;
                                case 1:
                                    labelText = "PawnConverterInvalidRace".Translate(def.label);
                                    break;
                                case 2:
                                    labelText = "PawnConverterInvalidGender".Translate(def.label);
                                    break;
                                case 3:
                                    labelText = "PawnConverterInvalidHediffs".Translate(def.label);
                                    break;
                                default:
                                    labelText = "PawnConverterNeedsResearch".Translate(def.label, def.requiredResearch.LabelCap);
                                    break;
                            }
                        }
                        else
                        {
                            labelText = "PawnConverterConvert".Translate(def.label);
                        }

                        FloatMenuOption option = new FloatMenuOption(labelText,
                        (Action)delegate ()
                        {
                            if (!disabled)
                            {
                                Job val2 = new Job(ConverterDefOf.EnterConverter, this);
                                ReservationUtility.Reserve(myPawn, this, val2);
                                myPawn.jobs.TryTakeOrderedJob(val2);
                                chosenRecipe = def;
                                ICookingTime = chosenRecipe.conversionTime;
                            }
                        });

                        option.Disabled = disabled;
                        floatMenuOptions.Add(option);
                    }
                }

                if (floatMenuOptions.Count > 0)
                {
                    return floatMenuOptions;
                }

                //Old Shit
                /** FloatMenuOption item6 = new FloatMenuOption(Translator.Translate("EnterConverter"), (Action)delegate
                {
                    Job val2 = new Job(ConverterDefOf.EnterConverter, this);
                    ReservationUtility.Reserve(myPawn, this, val2);
                    myPawn.jobs.TryTakeOrderedJob(val2);
                }, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item6
                }; **/
            }
            return null;
        }

        public bool HasRequiredHediffs(Pawn pawn, PawnConvertingDef recipe)
        {
            if(recipe.requiredHediffs == null)
            {
                return true;
            }
            if(recipe.requiredHediffs.All(x => pawn.health.hediffSet.HasHediff(x, false)))
            {
                if (recipe.hediffSeverityMatters)
                {
                    IEnumerable<HediffDef> enumerable = from def in recipe.requiredHediffs
                                                        where pawn.health.hediffSet.HasHediff(def, false)
                                                        select def;
                    foreach (HediffDef current in enumerable)
                    {
                        if (pawn.health.hediffSet.GetFirstHediffOfDef(current, false) != null)
                        {
                            if (pawn.health.hediffSet.GetFirstHediffOfDef(current, false).Severity < recipe.requiredHediffSeverity)
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
                if (item is Pawn val && this.chosenRecipe != null)
                {
                    if (item.def.defName == "Human")
                    {
                        convertedContainer.TryAdd(Util_PawnConvert.PawnConversion(Util_PawnConvert.HumanPawnConversion(val, this.chosenRecipe), this.chosenRecipe));
                    }
                    else
                    {
                        convertedContainer.TryAdd(Util_PawnConvert.PawnConversion(val, this.chosenRecipe));
                    }
                }
            }
            this.innerContainer.ClearAndDestroyContents();
            this.innerContainer = convertedContainer;
            this.chosenRecipe = null;
        }

        public override void Tick()
        {
            if (converterProperties.requiresPower)
            {
                if (powerComp.PowerOn)
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
                else
                {
                    ICookingTicking = 0;
                    if (this.HasAnyContents)
                    {
                        this.EjectContents();
                    }
                }
            }
            if (!converterProperties.requiresPower)
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
                SoundStarter.PlayOneShot(converterProperties.finishingSound, SoundInfo.OnCamera());
            }
            ICookingTicking = 0;
            this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near);
            this.contentsKnown = true;
        }

        public override void Draw()
        {
            base.Draw();
            if (converterProperties.timerBarEnabled)
            {
                DrawTimerBar();
            }
        }

        public void DrawTimerBar()
        {
            //Replaced Drawhelper with vanilla drawer here
            GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
            fillableBarRequest.preRotationOffset = converterProperties.timerBarOffset;
            fillableBarRequest.size = converterProperties.timerBarSize;
            fillableBarRequest.fillPercent = (float)ICookingTicking / (float)ICookingTime;
            fillableBarRequest.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(converterProperties.timerBarFill);
            fillableBarRequest.unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(converterProperties.timerBarUnfill);
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

        public static Building_Converter FindConverterFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
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
