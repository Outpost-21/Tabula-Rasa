﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using O21Toolbox.Utility;

namespace O21Toolbox.SlotLoadable
{
    public class FloatMenu_SlotLoadable : HumanlikeOrdersUtility.FloatMenuPatch
    {
        public override IEnumerable<KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
            GetFloatMenus()
        {
            var FloatMenus = new List<KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>();

            var curCondition = new HumanlikeOrdersUtility._Condition(HumanlikeOrdersUtility._ConditionType.ThingHasComp, typeof(Comp_SlottedBonus));

            List<FloatMenuOption> CurFunc(Vector3 clickPos, Pawn pawn, Thing curThing)
            {
                //LogUtil.LogMessage("Patch is loaded");
                var opts = new List<FloatMenuOption>();
                List<IThingHolder> holders = new List<IThingHolder>();
                pawn.GetChildHolders(holders);
                var allThings = new List<Thing>();
                holders.ForEach(x => allThings.AddRange(x.GetDirectlyHeldThings().ToList()));
                foreach (var item in allThings)
                {
                    var compSlotLoadable = item.TryGetComp<Comp_SlotLoadable>();
                    if (compSlotLoadable != null)
                    {
                        var c = clickPos.ToIntVec3();
                        //var thingList = c.GetThingList(pawn.Map);

                        foreach (var slot in compSlotLoadable.Slots)
                        {
                            var loadableThing = (slot.CanLoad(curThing.def)) ? curThing : null;
                            if (loadableThing != null)
                            {
                                FloatMenuOption itemSlotLoadable;
                                var labelShort = loadableThing.Label;
                                if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" + "Incapable".Translate() + ")",
                                        null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else if (!pawn.CanReach(loadableThing, PathEndMode.ClosestTouch, Danger.Deadly))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" + "NoPath".Translate() + ")", null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else if (!pawn.CanReserve(loadableThing, 1))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" +
                                        "ReservedBy".Translate(pawn.Map.physicalInteractionReservationManager
                                            .FirstReserverOf(loadableThing).LabelShort) + ")", null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else
                                {
                                    var text2 = "Equip".Translate(labelShort);
                                    itemSlotLoadable = new FloatMenuOption(text2, delegate
                                    {
                                        loadableThing.SetForbidden(false, true);
                                        pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(
                                            Utility.JobDefOf.O21_GatherSlotItem,
                                            loadableThing));
                                        FleckMaker.Static(loadableThing.DrawPos, loadableThing.Map,
                                            FleckDefOf.FeedbackEquip, 1f);
                                        //PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
                                    }, MenuOptionPriority.High, null, null, 0f, null, null);
                                }
                                opts.Add(itemSlotLoadable);
                            }
                        }
                        return opts;
                    }
                }
                return opts;
            }

            KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>> curSec =
                new KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>(curCondition, CurFunc);
            FloatMenus.Add(curSec);
            return FloatMenus;
        }
    }
}
