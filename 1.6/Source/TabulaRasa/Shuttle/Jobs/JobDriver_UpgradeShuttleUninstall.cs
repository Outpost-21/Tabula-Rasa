using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace TabulaRasa.Shuttle
{
    public class JobDriver_UpgradeShuttleUninstall : JobDriver
    {
        public const TargetIndex target = TargetIndex.A;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve((Shuttle)job.GetTarget(target).Thing, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            Shuttle shuttle = (Shuttle)job.GetTarget(target).Thing;
            UpgradeDef upgrade = shuttle.CurrUninstall;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => shuttle == null || upgrade == null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil doUninstall = ToilMaker.MakeToil("DoUninstallWork");
            doUninstall.tickIntervalAction = delegate (int delta)
            {
                shuttle.DoUninstallWork(pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * (float)delta);
                if (shuttle.CurrUninstall == null)
                {
                    ReadyForNextToil();
                }
            };
            doUninstall.defaultCompleteMode = ToilCompleteMode.Never;
            doUninstall.WithProgressBar(target, () => (upgrade.workToInstall > 0f) ? (shuttle.uninstallWorkRemaining / upgrade.workToInstall) : 1f);
            doUninstall.FailOnCannotTouch(target, PathEndMode.Touch);
            doUninstall.activeSkill = () => SkillDefOf.Construction;
            yield return doUninstall;
        }
    }
}
