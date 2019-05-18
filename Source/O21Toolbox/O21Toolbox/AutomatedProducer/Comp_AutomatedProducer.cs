using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class Comp_AutomatedProducer : ThingComp
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

        private CompPowerTrader powerComp;

        private CompProperties_AutomatedProducer producerCompProps;

        private int nextProductionTick = -1;

        public ProducerStatus currentStatus = ProducerStatus.idle;

        public string CurrentStatusLabel()
        {
            string result = "";
            if (currentStatus == ProducerStatus.idle)
            {
                result = "Idle";
            }
            else if(currentStatus == ProducerStatus.awaitingResources)
            {
                result = "Awaiting Resources: ";
                foreach(ThingDefCount thing in currentRecipe.costs)
                {
                    result = result + "\n" + thing.Count.ToString() + " " + thing.ThingDef.LabelCap;
                }
            }
            else if(currentStatus == ProducerStatus.working)
            {
                result = "Working";
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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.producerCompProps = this.Props;
        }

        public override void CompTick()
        {
            base.CompTick();
            GetProducerStatus();

            if(currentRecipe != null && nextProductionTick < 0)
            {
                ResetProductionTick();
            }

            if (currentRecipe != null && ShouldProduceThisTick())
            {
                ProduceFromRecipe();
            }
        }

        public void ProduceFromRecipe()
        {
            if (HasCosts())
            {
                
            }
            RepeatRecipe();
            ResetProductionTick();
        }

        public void RepeatRecipe()
        {
            if(currentRecipe != null && !repeatCurrentRecipe)
            {
                currentRecipe = null;
            }
        }

        public void ResetProductionTick()
        {
            int result = -1;
            if (currentRecipe != null)
            {
                if (currentRecipe.randomTickCost)
                {
                    result = Current.Game.tickManager.TicksGame + UnityEngine.Random.Range(currentRecipe.randomCostMin, currentRecipe.randomCostMax);

                }
                else
                {
                    result = Current.Game.tickManager.TicksGame + (int)(currentRecipe.baseTickCost * producerCompProps.craftingTimeMultiplier);
                }
            }
            nextProductionTick = result;
        }

        public bool ShouldProduceThisTick()
        {
            if (powerComp != null)
            {
                if (powerComp.PowerOn && Current.Game.tickManager.TicksGame >= nextProductionTick)
                {
                    return true;
                }
            }
            else if (Current.Game.tickManager.TicksGame >= nextProductionTick)
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
            if (!HasCosts())
            {
                currentStatus = ProducerStatus.awaitingResources;
            }
            if (IsWorking())
            {
                currentStatus = ProducerStatus.working;
            }
            currentStatus = ProducerStatus.producing;
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
                return false;
            }
            return true;
        }

        public bool IsWorking()
        {
            if(Current.Game.tickManager.TicksGame >= nextProductionTick)
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
