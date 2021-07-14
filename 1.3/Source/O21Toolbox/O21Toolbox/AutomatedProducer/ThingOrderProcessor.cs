using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    /// <summary>
    /// Helps in processing multiplie Thing Order Requests.
    /// </summary>
    public class ThingOrderProcessor : IExposable
    {
        /// <summary>
        /// Inventory to check for.
        /// </summary>
        public ThingOwner thingHolder;

        /// <summary>
        /// Storage settings to take in account for Nutrition.
        /// </summary>
        public StorageSettings storageSettings;

        /// <summary>
        /// List of requested. And ideal.
        /// </summary>
        public List<ThingOrderRequest> requestedItems = new List<ThingOrderRequest>();

        public ThingOrderProcessor()
        {

        }

        public ThingOrderProcessor(ThingOwner thingHolder)
        {
            this.thingHolder = thingHolder;
        }

        /// <summary>
        /// Gets all pending requests that need to be processed using ideal requests as a base.
        /// </summary>
        /// <returns>Pending requests or none.</returns>
        public IEnumerable<ThingOrderRequest> PendingRequests()
        {
            foreach(ThingOrderRequest idealRequest in requestedItems)
            {
                
                float totalItemCount = thingHolder.TotalStackCountOfDef(idealRequest.thingDef);
                if(totalItemCount < idealRequest.amount)
                {
                    ThingOrderRequest request = new ThingOrderRequest();
                    request.thingDef = idealRequest.thingDef;
                    request.amount = idealRequest.amount - totalItemCount;

                    yield return request;
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref requestedItems, "requestedItems", LookMode.Deep);
        }
    }
}
