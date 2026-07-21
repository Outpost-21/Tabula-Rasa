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
    public class WorkGiver_UpgradeShuttleInstall : WorkGiver_UpgradeShuttle
    {
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Shuttle shuttle)) { return false; }
            if (t.IsForbidden(pawn)) { return false; }
            if (!pawn.CanReserve(t, ignoreOtherReservations: forced)) { return false; }
            if (!shuttle.DueInstall) { return false; }
            UpgradeDef upgrade = shuttle.CurrInstall;
            if (upgrade == null) { return false; }
            if (!upgrade.costList.NullOrEmpty())
            {
                foreach (ThingDefCountClass defCount in upgrade.costList)
                {
                    if (!pawn.Map.itemAvailability.ThingsAvailableAnywhere(defCount.thingDef, defCount.count, pawn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_InstallShuttleUpgrade, t);
        }
    }
}
