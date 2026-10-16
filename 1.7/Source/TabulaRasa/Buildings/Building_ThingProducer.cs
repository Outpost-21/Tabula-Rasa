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
    public class Building_ThingProducer : Building
    {
        public CompPowerTrader powerComp;

        public DefModExt_ThingProducer producerProps;

        protected bool contentsKnown;

        public int storedThingCount = 0;

        public int currentWork;

        public float secondsTillNext => producerProps.productionTime.TicksToSeconds();

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
            if (Faction != null && Faction.IsPlayer)
            {
                contentsKnown = true;
            }
            currentWork = producerProps.productionTime;
        }

        public override void Tick()
        {
            base.Tick();
            if (!Spawned)
            {
                return;
            }
            if (storedThingCount < producerProps.maxThings)
            {
                if (currentWork <= 0)
                {
                    storedThingCount += 1;
                    currentWork = producerProps.productionTime;
                }

                if (currentWork > 0)
                {
                    currentWork--;
                }
            }
        }

        public bool HasAnyContents
        {
            get
            {
                return storedThingCount > 0;
            }
        }

        public string ContainedThing
        {
            get
            {
                return (storedThingCount != 0) + producerProps.thingDef.label;
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (!ReservationUtility.CanReserve(myPawn, this, 1))
            {
                FloatMenuOption item2 = new FloatMenuOption("CannotUseReserved".Translate(), null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!ReachabilityUtility.CanReach(myPawn, this, PathEndMode.OnCell, Danger.Some, false))
            {
                FloatMenuOption item3 = new FloatMenuOption("CannotUseNoPath".Translate(), null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (storedThingCount <= 0)
            {
                FloatMenuOption item3 = new FloatMenuOption("No available " + producerProps.thingDef.label, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (storedThingCount > 0)
            {
                FloatMenuOption item4 = new FloatMenuOption(producerProps.retrievalString.Translate(producerProps.thingDef.label), delegate
                {
                    Job val2 = new Job(TabulaRasaDefOf.TabulaRasa_TakeFromProducer, this);
                    ReservationUtility.Reserve(myPawn, this, val2);
                    myPawn.jobs.TryTakeOrderedJob(val2);
                });
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
                str = storedThingCount + "x " + producerProps.thingDef.label;
            }
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + "Contains" + ": " + str.CapitalizeFirst();
        }

        public void TakeItem(Pawn doer)
        {
            if (storedThingCount > 0)
            {
                Thing thing = ThingMaker.MakeThing(producerProps.thingDef, null);
                GenPlace.TryPlaceThing(thing, doer.Position, doer.Map, ThingPlaceMode.Near);
                storedThingCount -= 1;
            }
        }
    }
}
