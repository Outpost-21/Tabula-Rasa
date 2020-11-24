using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.MoonCycle
{
    public class WorldComponent_MoonCycle : WorldComponent
    {
        public List<Moon> moons;
        GameCondition moonCycleGC = null;
        public int ticksUntilFullMoon = -1;
        public int ticksPerMoonCycle = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksUntilFullMoon, "ticksUntilFullMoon", -1, false);
            //Scribe_Deep.Look<GameCondition>(ref moonCycleGC, "gcMoonCycle");
            Scribe_Collections.Look<Moon>(ref moons, "moons", LookMode.Deep, new object[0]);
        }

        public WorldComponent_MoonCycle(World world) : base(world)
        {
            if (moons.NullOrEmpty())
            {
                GenerateMoons(world);
            }
        }

        public override void WorldComponentTick()
        {
            if (!O21ToolboxMod.settings.moonCycleEnabled)
            {
                if (Find.World.GameConditionManager.GetActiveCondition<GameCondition_MoonCycle>() != null)
                {
                    Find.World.GameConditionManager.GetActiveCondition<GameCondition_MoonCycle>().End();
                    moonCycleGC = null;
                }
                return;
            }
            if(moonCycleGC != null)
            {
                moonCycleGC.End();
            }
            moons = null;
            //base.WorldComponentTick();
            //if (!moons.NullOrEmpty())
            //{
            //    foreach (Moon moon in moons)
            //    {
            //        moon.Tick();
            //    }
            //}
            //if (moonCycleGC == null)
            //{
            //    if(Find.World.GameConditionManager.GetActiveCondition<GameCondition_MoonCycle>() == null)
            //    {
            //        moonCycleGC = new GameCondition_MoonCycle();
            //        moonCycleGC.Permanent = true;
            //        Find.World.GameConditionManager.RegisterCondition(moonCycleGC);
            //    }
            //    else
            //    {
            //        moonCycleGC = Find.World.GameConditionManager.GetActiveCondition<GameCondition_MoonCycle>();
            //    }
            //}
        }

        public void AdvanceOneDay()
        {
            if (!moons.NullOrEmpty())
            {
                foreach (Moon moon in moons)
                {
                    moon.AdvanceOneDay();
                }
            }
        }

        public void AdvanceOneQuadrum()
        {
            if (!moons.NullOrEmpty())
            {
                foreach(Moon moon in moons)
                {
                    moon.AdvanceOneQuadrum();
                }
            }
        }

        public void GenerateMoons(World world)
        {
            if (moons == null)
            {
                moons = new List<Moon>();
            }
            int numMoons = 1;
            float val = Rand.Value;
            if (val > 0.98)
            {
                numMoons = Rand.Range(4, 6);
            }
            else if (val > 0.7) 
            {
                numMoons = 3; 
            }
            else if (val > 0.4) 
            { 
                numMoons = 2; 
            }
            for (int i = 0; i < numMoons; i++)
            {
                int uniqueID = 1;
                if (moons.Any()) 
                {
                    uniqueID = moons.Max((Moon o) => o.UniqueID) + 1;
                }

                moons.Add(new Moon(uniqueID, world, Rand.Range(350000 * (i + 1), 600000 * (i + 1))));
            }
        }

        public void DebugTriggerNextFullMoon()
        {
            Moon soonestMoon = null;
            if(!moons.NullOrEmpty() && moonCycleGC != null)
            {
                soonestMoon = moons.MinBy(x => x.DaysUntilFull);
                for (int i = 0; i < soonestMoon.DaysUntilFull; i++)
                {
                    AdvanceOneDay();
                }
                soonestMoon.FullMoonIncident();
            }
        }

        public void DebugRegenerateMoons(World world)
        {
            moons = null;
            GenerateMoons(world);
            Messages.Message("DEBUG :: Moons Regenerated", MessageTypeDefOf.TaskCompletion);
        }
    }
}
