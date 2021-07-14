using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    /// <summary>
    /// Properties for WorkGivers related to Automated Producers.
    /// </summary>
    public class WorkGiver_Properties_AutomatedProducer : DefModExtension
    {
        /// <summary>
        /// ThingDef to scan for.
        /// </summary>
        public ThingDef defToScan;

        /// <summary>
        /// Fill Job to give.
        /// </summary>
        public JobDef fillJob;
    }
}
