using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa
{
    /// <summary>
    /// Marks a Thing as a location to hibernate at for MachineLikes.
    /// </summary>
    public class Comp_Hibernation : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (selPawn.def.HasModExtension<DefModExt_EnergyNeed>())
            {
                DefModExt_EnergyNeed modExt = selPawn.def.GetModExtension<DefModExt_EnergyNeed>();
                if (modExt.canHibernate)
                {
                    if (selPawn.CanReserveAndReach(parent, PathEndMode.OnCell, Danger.Deadly))
                    {
                        if (parent.TryGetComp<CompPowerTrader>() is CompPowerTrader power)
                        {
                            if (power.PowerOn)
                            {
                                FloatMenuOption option = new FloatMenuOption("TabulaRasa.Hibernate".Translate(selPawn.Name.ToStringShort),
                                delegate ()
                                {
                                    //Give hibernation Job.
                                    selPawn.jobs.TryTakeOrderedJob(new Job(TabulaRasaDefOf.TabulaRasa_Hibernate, parent), JobTag.Misc);
                                });
                                yield return option;
                            }
                            else
                            {
                                FloatMenuOption option = new FloatMenuOption("TabulaRasa.HibernateFailNoPower".Translate(selPawn.Name.ToStringShort, parent.LabelCap), null);
                                option.Disabled = true;
                                yield return option;
                            }
                        }
                        else
                        {
                            FloatMenuOption option = new FloatMenuOption("TabulaRasa.Hibernate".Translate(selPawn.Name.ToStringShort),
                            delegate ()
                            {
                                //Give hibernation Job.
                                selPawn.jobs.TryTakeOrderedJob(new Job(TabulaRasaDefOf.TabulaRasa_Hibernate, parent), JobTag.Misc);
                            });
                            yield return option;
                        }
                    }
                    else
                    {
                        FloatMenuOption option = new FloatMenuOption("TabulaRasa.HibernateFailReserveOrReach".Translate(selPawn.Name.ToStringShort, parent.LabelCap), null);
                        option.Disabled = true;
                        yield return option;
                    }
                }
            }
            else
            {
                FloatMenuOption option = new FloatMenuOption("TabulaRasa.HibernateFail".Translate(selPawn.Name.ToStringShort), null);
                option.Disabled = true;
                yield return option;
            }
        }
    }
}
