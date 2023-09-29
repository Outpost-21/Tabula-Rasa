using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    //TODO: Remove next RW update
    public class Comp_EnergySource : ThingComp
    {
        public CompProperties_EnergySource Props => props as CompProperties_EnergySource;

        public virtual void RechargeEnergyNeed(Pawn targetPawn)
        {
            Need_Energy energyNeed = targetPawn.needs.TryGetNeed<Need_Energy>();
            if (energyNeed != null)
            {
                float finalEnergyGain = parent.stackCount * Props.energyGiven;
                energyNeed.CurLevel += finalEnergyGain;
                if(Props.outputThing != null)
                {
                    Thing thing = ThingMaker.MakeThing(Props.outputThing);
                    if (!GenPlace.TryPlaceThing(thing, targetPawn.Position, targetPawn.Map, ThingPlaceMode.Near))
                    {
                        LogUtil.LogError($"{targetPawn}, could not drop recipe product {thing} near {targetPawn.Position}");
                    }
                }
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            Need_Energy energyNeed = selPawn.needs.TryGetNeed<Need_Energy>();
            if (energyNeed != null)
            {
                int thingCount = (int)Math.Ceiling((energyNeed.MaxLevel - energyNeed.CurLevel) / Props.energyGiven);

                if (thingCount > 0)
                {
                    FloatMenuOption floatMenuOption = new FloatMenuOption("TabulaRasa.ConsumeEnergySource".Translate(parent.LabelCap), () => selPawn.jobs.TryTakeOrderedJob(new Verse.AI.Job(TabulaRasaDefOf.TabulaRasa_ConsumeEnergySource, new LocalTargetInfo(parent)) { count = thingCount }), MenuOptionPriority.Default, null, parent);

                    yield return floatMenuOption;
                }
            }
        }
    }
}
