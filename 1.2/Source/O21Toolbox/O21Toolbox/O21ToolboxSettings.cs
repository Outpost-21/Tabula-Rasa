using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class O21ToolboxSettings : ModSettings
    {
        // General
        public bool animationsEnabled = true;
        public bool modUpdates = true;
        public float humanSpawnWeight = 20;

        // Jetpacks
        public bool roofPunch = true;
        public bool jetpackAutoRefuel = true;

        // UpdateListings
        public List<string> markedAsSeen = new List<String>();

        public override void ExposeData()
        {
            // General
            Scribe_Values.Look(ref this.animationsEnabled, "animationsEnabled", true);
            Scribe_Values.Look(ref this.modUpdates, "modUpdates", true);
            Scribe_Values.Look(ref this.humanSpawnWeight, "humanSpawnWeight", 20);

            // Jetpacks
            Scribe_Values.Look(ref this.roofPunch, "roofPunch", true);
            Scribe_Values.Look(ref this.jetpackAutoRefuel, "jetpackAutoRefuel", true);

            // UpdateListings
            Scribe_Collections.Look(ref this.markedAsSeen, "markedAsSeen");
        }
    }
}
