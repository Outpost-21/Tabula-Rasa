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
    public class FloatMenuProvider_SlotLoadable : FloatMenuOptionProvider
    {
        public override bool Multiselect => false;

        public override bool RequiresManipulation => true;

        public override bool Drafted => true;

        public override bool Undrafted => true;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            return thing.HasComp<Comp_SlotLoadable>();
        }

        public override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            Pawn pawn = context.FirstSelectedPawn;
            if (pawn == null) { return null; }
            if (clickedThing == null) { return null; }

            FloatMenuOption itemSlotLoadable;
            var labelShort = clickedThing.Label;
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                itemSlotLoadable = new FloatMenuOption(
                    "CannotEquip".Translate(labelShort) + " (" + "Incapable".Translate() + ")",
                    null,
                    MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!pawn.CanReach(clickedThing, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                itemSlotLoadable = new FloatMenuOption(
                    "CannotEquip".Translate(labelShort) + " (" + "NoPath".Translate() + ")", null,
                    MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!pawn.CanReserve(clickedThing, 1))
            {
                itemSlotLoadable = new FloatMenuOption(
                    "CannotEquip".Translate(labelShort) + " (" +
                    "ReservedBy".Translate(pawn.Map.physicalInteractionReservationManager
                        .FirstReserverOf(clickedThing).LabelShort) + ")", null,
                    MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else
            {
                var text2 = "Equip".Translate(labelShort);
                itemSlotLoadable = new FloatMenuOption(text2, delegate
                {
                    clickedThing.SetForbidden(false, true);
                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(
                        TabulaRasaDefOf.TabulaRasa_GatherSlotItem,
                        clickedThing));
                    FleckMaker.Static(clickedThing.DrawPos, clickedThing.Map,
                        FleckDefOf.FeedbackEquip, 1f);
                }, MenuOptionPriority.High, null, null, 0f, null, null);
            }
            return itemSlotLoadable;
        }
    }
}
