using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.ThingProducer
{
    public class Building_ThingProducer : Building
    {
        [DefOf]
        public static class ProducerDefOf
        {
            /// <summary>
            /// Job def telling pawns to take an item from the producer.
            /// </summary>
            public static JobDef O21TakeFromProducer;
        }

        public CompPowerTrader powerComp;

        public DefModExt_ThingProducer producerProps;

        protected bool contentsKnown;

        public int storedThingCount = 0;

        public int currentWork;

        public float secondsTillNext => this.producerProps.productionTime.TicksToSeconds();

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref currentWork, "currentWork");
            Scribe_Values.Look(ref contentsKnown, "contentsKnown");
            Scribe_Values.Look(ref storedThingCount, "storedThingCount");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            producerProps = def.GetModExtension<DefModExt_ThingProducer>();
            if (base.Faction != null && base.Faction.IsPlayer)
            {
                this.contentsKnown = true;
            }
            currentWork = producerProps.productionTime;
        }

        public override void Tick()
        {
            base.Tick();
            if(storedThingCount < producerProps.maxThings)
            {
                if(currentWork <= 0)
                {
                    storedThingCount += 1;
                    currentWork = producerProps.productionTime;
                }

                if(currentWork > 0)
                {
                    currentWork--;
                }
            }
        }

        public bool HasAnyContents
        {
            get
            {
                return this.storedThingCount > 0;
            }
        }

        public string ContainedThing
        {
            get
            {
                return (this.storedThingCount != 0) + this.producerProps.thingDef.label;
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (!ReservationUtility.CanReserve(myPawn, this, 1))
            {
                FloatMenuOption item2 = new FloatMenuOption("CannotUseReserved".Translate(), (Action)null, MenuOptionPriority.Default, (Action<Rect>)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!ReachabilityUtility.CanReach(myPawn, this, PathEndMode.OnCell, Danger.Some, false))
            {
                FloatMenuOption item3 = new FloatMenuOption("CannotUseNoPath".Translate(), (Action)null, MenuOptionPriority.Default, (Action<Rect>)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if(this.storedThingCount <= 0)
            {
                FloatMenuOption item3 = new FloatMenuOption("No available " + this.producerProps.thingDef.label, (Action)null, MenuOptionPriority.Default, (Action<Rect>)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (this.storedThingCount > 0)
            {
                FloatMenuOption item4 = new FloatMenuOption(this.producerProps.retrievalString, (Action)delegate
                {
                    Job val2 = new Job(ProducerDefOf.O21TakeFromProducer, this);
                    ReservationUtility.Reserve(myPawn, this, val2);
                    myPawn.jobs.TryTakeOrderedJob(val2);
                }, MenuOptionPriority.Default, (Action<Rect>)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item4
                };
            }
            return null;
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str;
            if (!this.contentsKnown)
            {
                str = "Contents Unknown";
            }
            else
            {
                str = this.storedThingCount + "x " + this.producerProps.thingDef.label;
            }
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + "Contains" + ": " + str.CapitalizeFirst();
        }

        public void TakeItem(Pawn doer)
        {
            if (this.storedThingCount > 0)
            {
                Thing thing = ThingMaker.MakeThing(this.producerProps.thingDef, null);
                GenPlace.TryPlaceThing(thing, doer.Position, doer.Map, ThingPlaceMode.Near, null, null);
                this.storedThingCount -= 1;
            }
        }
    }
}
