using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class Comp_AutomatedProducer : ThingComp, IThingHolder
    {
        public CompProperties_AutomatedProducer Props
        {
            get
            {
                return (CompProperties_AutomatedProducer)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        /// <summary>
        /// Stored ingredients for use in producing one pawn.
        /// </summary>
        public ThingOwner<Thing> ingredients = new ThingOwner<Thing>();

        /// <summary>
        /// Convenience class for setting what resources is needed to make one of the selected recipe.
        /// </summary>
        public ThingOrderProcessor orderProcessor;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            //None
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return ingredients;
        }

        private CompPowerTrader powerComp;

        private CompProperties_AutomatedProducer producerCompProps;

        private int workTick = -50;
        private int workTickMax = 0;

        public ProducerStatus currentStatus = ProducerStatus.idle;

        public string CurrentStatusLabel()
        {
            string result = " ";
            if (currentStatus == ProducerStatus.idle)
            {
                result = "Idle";
            }
            else if(currentStatus == ProducerStatus.awaitingResources && orderProcessor.PendingRequests() != null)
            {
                result = "Awaiting Resources: ";
                foreach(ThingOrderRequest thing in orderProcessor.PendingRequests())
                {
                    result = result + "\n" + thing.amount.ToString() + " " + thing.thingDef.LabelCap;
                }
            }
            else if(currentStatus == ProducerStatus.working)
            {
                result = "Working: " + (100 - (100 * workTick / workTickMax)) + "%";
            }
            else if(currentStatus == ProducerStatus.producing)
            {
                result = "Producing";
            }
            return result;
        }

        public string RepeatString()
        {
            if (repeatCurrentRecipe)
            {
                return "Repeat: True";
            }
            else
            {
                return "Repeat: False";
            }
        }

        public RecipeDef_Automated currentRecipe = null;

        public bool repeatCurrentRecipe = false;

        public bool hasOrder = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.producerCompProps = this.Props;

            if(workTick <= 0)
            {
                ResetWorkTick();
            }

            if (!respawningAfterLoad)
            {
                orderProcessor = new ThingOrderProcessor(ingredients);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            this.GetProducerStatus();

            if (this.HasRecipe() && !this.hasOrder)
            {
                //Update costs.
                this.orderProcessor.requestedItems.Clear();

                foreach (ThingDefCountClass cost in this.currentRecipe.costs)
                {
                    ThingOrderRequest costCopy = new ThingOrderRequest();
                    costCopy.thingDef = cost.thingDef;
                    costCopy.amount = cost.count;

                    this.orderProcessor.requestedItems.Add(costCopy);
                }

                this.hasOrder = true;
            }

            if (this.currentRecipe != null && this.IsWorking() && this.workTick <= 0)
            {
                this.ProduceFromRecipe();
            }

            if (IsWorking())
            {

                this.workTick--;
            }
        }

        public void ProduceFromRecipe()
        {
            if (currentRecipe != null && IsWorking())
            {
                foreach(ThingDefCount tdc in currentRecipe.products)
                {
                    Thing foundThing = null;
                    Thing producedThing = ThingMaker.MakeThing(tdc.ThingDef, null);

                    if(this.parent.InteractionCell != null)
                    {
                        if(this.parent.InteractionCell.GetFirstHaulable(this.parent.Map) != null)
                        {
                            foundThing = this.parent.InteractionCell.GetFirstHaulable(this.parent.Map);
                        }

                        if(foundThing != null && foundThing.def == producedThing.def && this.Props.allowFullStack)
                        {
                            if(foundThing.def.stackLimit >= (foundThing.stackCount + tdc.Count) && this.Props.allowOverflow)
                            {
                                foundThing.stackCount = foundThing.def.stackLimit;
                                break;
                            }
                            else if(foundThing.def.stackLimit <= (foundThing.stackCount + tdc.Count) && !this.Props.allowOverflow)
                            {
                                producedThing.stackCount = tdc.Count;
                            }
                        }
                        else if(foundThing != null && foundThing.def != producedThing.def)
                        {
                            if (this.Props.allowOverflow)
                            {
                                producedThing.stackCount = tdc.Count;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (foundThing == null)
                        {
                            producedThing.stackCount = tdc.Count;
                        }

                        GenPlace.TryPlaceThing(producedThing, this.parent.InteractionCell, this.parent.Map, ThingPlaceMode.Near, null, null);
                        break;
                    }
                    Log.Error("Interaction Cell not defined on " + this.parent.def.defName + ", output requires an interaction cell.");
                    return;
                }
            }
            else
            {
                return;
            }
            ingredients.ClearAndDestroyContents();
            RepeatRecipe();
            ResetWorkTick();
        }

        public void RepeatRecipe()
        {
            if(currentRecipe != null && !repeatCurrentRecipe)
            {
                currentRecipe = null;
            }
        }

        public void ResetWorkTick()
        {
            if(orderProcessor != null)
            {
                orderProcessor.requestedItems.Clear();
                hasOrder = false;
            }
            int result = -50;
            if (currentRecipe != null)
            {
                if (currentRecipe.randomTickCost)
                {
                    result = UnityEngine.Random.Range(currentRecipe.randomCostMin, currentRecipe.randomCostMax);

                }
                else
                {
                    result = (int)(currentRecipe.baseTickCost * producerCompProps.craftingTimeMultiplier);
                }
                Log.Message("Recipe Work: " + result);
            }
            workTick = result;
            workTickMax = result;
        }

        public void CancelRecipe()
        {
            ingredients.TryDropAll(this.parent.InteractionCell, this.parent.Map, ThingPlaceMode.Near);
            this.currentRecipe = null;
        }

        public bool ShouldProduceThisTick()
        {
            if (powerComp != null)
            {
                if (powerComp.PowerOn && Current.Game.tickManager.TicksGame >= workTick)
                {
                    return true;
                }
            }
            else if (Current.Game.tickManager.TicksGame >= workTick)
            {
                return true;
            }
            return false;
        }

        public void GetProducerStatus()
        {
            if (!IsPowered() || !HasRecipe())
            {
                currentStatus = ProducerStatus.idle;
            }
            else if ((HasCosts() || AwaitingCosts()) && !HasIngredients())
            {
                currentStatus = ProducerStatus.awaitingResources;
            }
            else if (IsWorking())
            {
                currentStatus = ProducerStatus.working;
            }
            else
            {
                currentStatus = ProducerStatus.producing;
            }
        }

        public bool IsPowered()
        {
            if(powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }
            return true;
        }

        public bool HasRecipe()
        {
            if (currentRecipe != null)
            {
                return true;
            }
            return false;
        }

        public bool HasCosts()
        {
            if(HasRecipe() && currentRecipe.costs != null)
            {
                return true;
            }
            return false;
        }

        public bool AwaitingCosts()
        {
            if (HasCosts() && orderProcessor.PendingRequests().Count() > 0)
            {
                return true;
            }
            return false;
        }

        public bool HasIngredients()
        {
            if (!HasCosts() || (orderProcessor.PendingRequests() == null || (orderProcessor.PendingRequests() != null && orderProcessor.PendingRequests().Count() <= 0)))
            {
                return true;
            }
            return false;
        }

        public bool IsWorking()
        {
            if(HasIngredients() && IsPowered() && workTick > -2)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Current status of the producer.
    /// </summary>
    public enum ProducerStatus
    {
        /// <summary>
        /// Idle means it has no recipe.
        /// </summary>
        idle,
        /// <summary>
        /// AwaitingResources means it has a recipe but currently doesn't have the resources to craft.
        /// </summary>
        awaitingResources,
        /// <summary>
        /// Working means it has both a recipe and the resources to craft and is currently processing.
        /// Resources are stored inside a buffer once this stage is active, cancelling will drop the resources.
        /// </summary>
        working,
        /// <summary>
        /// Likely never seen as it happens quickly, Producing means it is currently spitting out the resulting products.
        /// </summary>
        producing
    }
}
