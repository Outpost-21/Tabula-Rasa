using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Marks a Thing as a location to hibernate at for MachineLikes.
    /// </summary>
    public class Comp_Hibernation : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if(selPawn.TryGetComp<Comp_EnergyTracker>() is Comp_EnergyTracker energyTracker && energyTracker.EnergyProperties is CompProperties_EnergyTracker props && props.canHibernate)
            {
                if(selPawn.CanReserveAndReach(parent, PathEndMode.OnCell, Danger.Deadly))
                {
                    if(parent.TryGetComp<CompPowerTrader>() is CompPowerTrader power)
                    {
                        if(power.PowerOn)
                        {
                            FloatMenuOption option = new FloatMenuOption("O21MachinelikeHibernate".Translate(selPawn.Name.ToStringShort),
                            delegate ()
                            {
                                //Give hibernation Job.
                                selPawn.jobs.TryTakeOrderedJob(new Job(props.hibernationJob, parent), JobTag.Misc);
                            });
                            yield return option;
                        }
                        else
                        {
                            FloatMenuOption option = new FloatMenuOption("O21MachinelikeHibernateFailNoPower".Translate(selPawn.Name.ToStringShort, parent.LabelCap), null);
                            option.Disabled = true;
                            yield return option;
                        }
                    }
                    else
                    {
                        FloatMenuOption option = new FloatMenuOption("O21MachinelikeHibernate".Translate(selPawn.Name.ToStringShort),
                        delegate ()
                        {
                            //Give hibernation Job.
                            selPawn.jobs.TryTakeOrderedJob(new Job(props.hibernationJob, parent), JobTag.Misc);
                        });
                        yield return option;
                    }
                }
                else
                {
                    FloatMenuOption option = new FloatMenuOption("O21MachinelikeHibernateFailReserveOrReach".Translate(selPawn.Name.ToStringShort, parent.LabelCap), null);
                    option.Disabled = true;
                    yield return option;
                }
            }
            else
            {
                FloatMenuOption option = new FloatMenuOption("O21MachinelikeHibernateFail".Translate(selPawn.Name.ToStringShort), null);
                option.Disabled = true;
                yield return option;
            }
        }
    }
}
