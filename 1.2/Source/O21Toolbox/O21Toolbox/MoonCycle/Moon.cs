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
    public class Moon : IExposable, ILoadReferenceable
    {
        private int uniqueID;
        private int ticksInCycle = -1;
        private int ticksLeftInCycle = -1;
        private string name = "";
        private World hostPlanet = null;

        public int UniqueID => uniqueID;
        public int DaysUntilFull => (int)((float)ticksLeftInCycle.TicksToDays());
        public string Name => name;

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.uniqueID, "uniqueID", 0, false);
            Scribe_Values.Look<string>(ref this.name, "name", "Moony McMoonFace");
            Scribe_Values.Look<int>(ref this.ticksInCycle, "ticksInCycle", -1);
            Scribe_Values.Look<int>(ref this.ticksLeftInCycle, "ticksLeftInCycle", -1);
        }

        public Moon() { }

        public Moon(int uniqueID, World world, int newTicks)
        {
            this.uniqueID = uniqueID;
            name = NameGenerator.GenerateName(RulePackDefOf.NamerWorld, (x => x != (hostPlanet?.info?.name ?? "") && x.Count() < 9), false);
            hostPlanet = world;
            ticksInCycle = newTicks;
            ticksLeftInCycle = ticksInCycle;
        }

        public void Tick()
        {
            if(ticksLeftInCycle < 0)
            {
                if(Find.CurrentMap is Map m)
                {
                    int time = GenLocalDate.HourInteger(m);
                    if(time <= 3 || time >= 21)
                    {
                        FullMoonIncident();
                    }
                    else
                    {
                        ticksLeftInCycle += GenDate.TicksPerHour;
                    }
                }
            }
            ticksLeftInCycle--;
        }

        public void AdvanceOneDay()
        {
            ticksLeftInCycle -= GenDate.TicksPerDay;
        }

        public void AdvanceOneQuadrum()
        {
            ticksLeftInCycle -= GenDate.TicksPerQuadrum;
        }

        public void FullMoonIncident()
        {
            ticksLeftInCycle = ticksInCycle;
            GameCondition_FullMoon fullMoon = new GameCondition_FullMoon(this);
            fullMoon.startTick = Find.TickManager.TicksGame;
            fullMoon.Duration = GenDate.TicksPerDay;
            Find.World.gameConditionManager.RegisterCondition(fullMoon);
        }

        public string GetUniqueLoadID()
        {
            return "Moon_" + this.uniqueID;
        }
    }
}
