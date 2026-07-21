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
    public class WorkGiver_UpgradeShuttleUninstall : WorkGiver_UpgradeShuttle
    {
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Shuttle shuttle)) { return false; }
            if (t.IsForbidden(pawn)) { return false; }
            if (!pawn.CanReserve(t, ignoreOtherReservations: forced)) { return false; }
            if (!shuttle.DueUninstall) { return false; }
            UpgradeDef upgrade = shuttle.CurrUninstall;
            if (upgrade == null) { return false; }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UninstallShuttleUpgrade, t);
        }
    }
}
