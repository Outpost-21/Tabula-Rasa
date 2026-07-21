using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace TabulaRasa.Shuttle
{
    public abstract class WorkGiver_UpgradeShuttle : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return UpgradeUtil.GetAllShuttlesOnMap(pawn.Map);
        }
    }
}
