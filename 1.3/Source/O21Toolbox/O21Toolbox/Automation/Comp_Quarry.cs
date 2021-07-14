using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Automation
{
    public class Comp_Quarry : ThingComp
    {
        public CompProperties_Quarry Props => (CompProperties_Quarry)props;

        public CompFlickable compFlickable;
        public CompPowerTrader compPower;
        public CompRefuelable compRefuelable;

        public MiningSettings mineableThings;

        public ThingDef currentlyMining;
        public int mineTicksRemaining = -1;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look<MiningSettings>(ref mineableThings, "mineableThings", new object[]
            {
                this
            });

            Scribe_Defs.Look<ThingDef>(ref currentlyMining, "currentlyMining");
            Scribe_Values.Look<int>(ref mineTicksRemaining, "mineTicksRemaining");
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            this.mineableThings = new MiningSettings(this);
            if(Props.defaultMiningSettings != null)
            {
                mineableThings.CopyFrom(Props.defaultMiningSettings);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            compFlickable = parent.TryGetComp<CompFlickable>();
            compPower = parent.TryGetComp<CompPowerTrader>();
            compRefuelable = parent.TryGetComp<CompRefuelable>();
        }

        public override void CompTick()
        {
            base.CompTick();
            if(compPower.PowerOn && compFlickable.SwitchIsOn)
            {
                if(mineTicksRemaining > 0)
                {
                    mineTicksRemaining--;
                }
                else if(currentlyMining != null)
                {
                    GenerateResult(currentlyMining);
                    currentlyMining = null;
                }
                else if(!mineableThings?.filter?.allowedDefs?.EnumerableNullOrEmpty() ?? false)
                {
                    currentlyMining = GetRandomAllowedMineable();
                    float timerBase = Props.tickCostMultiplier * (currentlyMining.BaseMarketValue * 1000f);
                    float timerDebuff = timerBase * (Props.costDebuffPercent * MiningUtility.cachedMineableThings.Except(mineableThings.filter.allowedDefs).Count());
                    mineTicksRemaining = Mathf.RoundToInt(timerBase + timerDebuff);
                }
            }
        }

        public void GenerateResult(ThingDef d)
        {
            if(d != null)
            {
                Thing t = ThingMaker.MakeThing(d);
                if(d.deepLumpSizeRange != null)
                {
                    t.stackCount = d.deepLumpSizeRange.RandomInRange;
                }

                GenPlace.TryPlaceThing(t, parent.InteractionCell, parent.Map, ThingPlaceMode.Near);
            }
        }

        public ThingDef GetRandomAllowedMineable()
        {
            return mineableThings.filter.allowedDefs.RandomElementByWeight(d => d.deepCommonality);
        }

        public override string CompInspectStringExtra()
        {
            if(currentlyMining != null)
            {
                return "Mining: " + currentlyMining.label + "\n" + "Time remaining: " + mineTicksRemaining.ToStringTicksToPeriod(true);
            }
            else
            {
                return "Mining Inactive";
            }

        }
    }
}
