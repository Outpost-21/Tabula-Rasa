using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace O21Toolbox.AutomatedProducer
{
    public class WorkGiver_AutomatedProducer : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(WorkGiverProperties.defToScan);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        private WorkGiver_Properties_AutomatedProducer intWorkGiverProperties = null;

        public WorkGiver_Properties_AutomatedProducer WorkGiverProperties
        {
            get
            {
                if(intWorkGiverProperties == null)
                {
                    intWorkGiverProperties = def.GetModExtension<WorkGiver_Properties_AutomatedProducer>();
                }

                return intWorkGiverProperties;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_AutomatedProducer autoProducer = t as Building_AutomatedProducer;

            if (autoProducer == null || autoProducer.TryGetComp<Comp_AutomatedProducer>().currentStatus != ProducerStatus.awaitingResources)
                return false;

            if (t.IsForbidden(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced))
            {
                return false;
            }

            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }

            IEnumerable<ThingOrderRequest> potentionalRequests = autoProducer.TryGetComp<Comp_AutomatedProducer>().orderProcessor.PendingRequests();
            bool validRequest = false;
            if (potentionalRequests != null)
            {
                foreach (ThingOrderRequest request in potentionalRequests)
                {
                    Thing ingredientThing = FindIngredient(pawn, autoProducer, request);
                    if(ingredientThing != null)
                    {
                        validRequest = true;
                        break;
                    }
                }
            }

            return validRequest;
        }

        public override Job JobOnThing(Pawn pawn, Thing crafterThing, bool forced = false)
        {
            Building_AutomatedProducer automatedProducer = crafterThing as Building_AutomatedProducer;

            IEnumerable<ThingOrderRequest> potentionalRequests = automatedProducer.TryGetComp<Comp_AutomatedProducer>().orderProcessor.PendingRequests();

            if (potentionalRequests != null)
            {
                foreach (ThingOrderRequest request in potentionalRequests)
                {
                    Thing ingredientThing = FindIngredient(pawn, automatedProducer, request);

                    if (ingredientThing != null)
                    {
                        return new Job(WorkGiverProperties.fillJob, ingredientThing, crafterThing)
                        {
                            count = (int)request.amount
                        };
                    }
                }
            }

            return null;
        }

        private Thing FindIngredient(Pawn pawn, Building_AutomatedProducer automatedProducer, ThingOrderRequest request)
        {
            if (request != null)
            {
                Predicate<Thing> extraPredicate = request.ExtraPredicate();
                Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && extraPredicate(x);
                Predicate<Thing> validator = predicate;

                return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, request.Request(), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
            }

            return null;
        }
    }
}
