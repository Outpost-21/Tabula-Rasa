using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.SlotLoadable;

namespace O21Toolbox.Utility
{
    [StaticConstructorOnStartup]
    public static class SlotLoadableUtility
    {
        // Grab slots of the thing if they exists. Returns null if none
        public static List<SlotLoadable.SlotLoadable> GetSlots(Thing someThing)
        {
            var compSlotLoadable = someThing.TryGetComp<Comp_SlotLoadable>();
            if (compSlotLoadable?.Slots?.Count > 0)
            {
                return compSlotLoadable.Slots;
            }
            return null;
        }

        // Get the thing's modificaiton to stat from it's slots
        public static float CheckThingSlotsForStatAugment(Thing slottedThing, StatDef stat)
        {
            var retval = 0.0f;
            var slots = GetSlots(slottedThing);

            if (slots != null)
            {
                foreach (var slot in slots)
                {
                    if (!slot.IsEmpty())
                    {
                        var slottable = slot.SlotOccupant;
                        retval += DetermineSlottableStatAugment(slottable, stat);
                    }
                }
            }
            return retval;
        }

        public static float DetermineSlottableStatAugment(Thing slottable, StatDef stat)
        {
            var retval = 0.0f;
            var slotBonus = slottable.TryGetComp<Comp_SlottedBonus>();
            if (slotBonus?.Props?.statModifiers != null)
            {
                foreach (var thisStat in slotBonus.Props.statModifiers)
                {
                    if (thisStat.stat == stat)
                    {
                        retval += thisStat.value;

                        // apply stats parts from Slottable
                        if (!stat.parts.NullOrEmpty())
                        {
                            var req = StatRequest.For(slottable);
                            for (var i = 0; i < stat.parts.Count; i++)
                            {
                                stat.parts[i].TransformValue(req, ref retval);
                            }
                        }
                    }
                }
            }

            return retval;
        }
    }
}
