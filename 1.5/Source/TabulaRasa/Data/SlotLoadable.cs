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
    public class SlotLoadable : Thing, IThingHolder
    {
        public SlotLoadable()
        {
        }

        public SlotLoadable(Thing newOwner)
        {
            var def = this.def as SlotLoadableDef;
            slottableThingDefs = def.slottableThingDefs;
            owner = newOwner;
            ThingIDMaker.GiveIDTo(this);
            slot = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public SlotLoadable(SlotLoadableDef xmlDef, Thing newOwner)
        {
            def = xmlDef;
            slottableThingDefs = xmlDef.slottableThingDefs;
            owner = newOwner;
            ThingIDMaker.GiveIDTo(this);
            slot = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public Texture2D SlotIcon()
        {
            if (SlotOccupant != null)
            {
                if (SlotOccupant.def != null)
                {
                    return SlotOccupant.def.uiIcon;
                }
            }
            return null;
        }

        public Color SlotColor()
        {
            if (SlotOccupant != null)
            {
                if (SlotOccupant.def != null)
                {
                    return SlotOccupant.def.graphic.Color;
                }
            }
            return Color.white;
        }

        public bool IsEmpty()
        {
            if (SlotOccupant != null)
            {
                return false;
            }
            return true;
        }

        public bool CanLoad(ThingDef defType)
        {
            if (slottableThingDefs != null)
            {
                if (slottableThingDefs.Contains(defType))
                {
                    return true;
                }
            }
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (thingIDNumber == -1)
                {
                    ThingIDMaker.GiveIDTo(this);
                }
            }
            Scribe_Deep.Look(ref slot, "slot", this);
            Scribe_Collections.Look(ref slottableThingDefs, "slottableThingDefs", LookMode.Undefined);
            Scribe_References.Look(ref owner, "owner");
        }

        private ThingOwner slot;

        public List<ThingDef> slottableThingDefs;

        public Thing owner;

        public Map GetMap()
        {
            return ParentMap;
        }

        public ThingOwner GetInnerContainer()
        {
            return slot;
        }

        public IntVec3 GetPosition()
        {
            return ParentLoc;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return slot;
        }

        public Thing SlotOccupant
        {
            get
            {
                if (slot.Count == 0)
                {
                    return null;
                }
                if (slot.Count > 1)
                {
                    Log.Error("ContainedThing used on a DropPodInfo holding > 1 thing.");
                }
                return slot[0];
            }
            set
            {
                slot.Clear();
                if (value.holdingOwner != null)
                {
                    var takenThing = value.holdingOwner.Take(value, 1);
                    value.DeSpawn();
                    slot.TryAdd(takenThing, true);
                }
                else
                {
                    slot.TryAdd(value, true);
                }
            }
        }

        public ThingOwner Slot
        {
            get => slot;
            set => slot = value;
        }

        public Pawn Holder
        {
            get
            {
                Pawn result = null;
                if (owner != null)
                {
                    var eq = owner.TryGetComp<CompEquippable>();
                    if (eq != null)
                    {
                        if (eq.PrimaryVerb != null)
                        {
                            var pawn = eq.PrimaryVerb.CasterPawn;
                            if (pawn != null)
                            {
                                if (pawn.Spawned)
                                {
                                    result = pawn;
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }

        public Map ParentMap
        {
            get
            {
                Map result = null;
                if (owner != null)
                {
                    if (Holder != null)
                    {
                        return Holder.Map;
                    }
                    return owner.Map;
                }
                return result;
            }
        }

        public IntVec3 ParentLoc
        {
            get
            {
                var result = IntVec3.Invalid;
                if (owner != null)
                {
                    if (Holder != null)
                    {
                        return Holder.Position;
                    }
                    return owner.Position;
                }
                return result;
            }
        }

        public List<ThingDef> SlottableTypes => slottableThingDefs;

        public virtual bool TryLoadSlot(Thing thingToLoad, bool emptyIfFilled = false)
        {
            if (SlotOccupant != null && emptyIfFilled || SlotOccupant == null)
            {
                TryEmptySlot();
                if (thingToLoad != null)
                {
                    if (slottableThingDefs != null)
                    {
                        if (slottableThingDefs.Contains(thingToLoad.def))
                        {
                            SlotOccupant = thingToLoad;
                            return true;
                        }
                    }
                }
            }
            else
            {
                Messages.Message(string.Format("{0}'s slot is already filled", new object[]
                {
                    owner.Label
                }), MessageTypeDefOf.RejectInput);
            }
            return false;
        }

        public virtual bool TryEmptySlot()
        {
            if (!CanEmptySlot()) return false;
            return slot.TryDropAll(ParentLoc, ParentMap, ThingPlaceMode.Near);
        }

        public virtual bool CanEmptySlot()
        {
            return true;
        }
    }
}
