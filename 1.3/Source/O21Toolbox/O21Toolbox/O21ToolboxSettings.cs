using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

//using O21Toolbox.Background;

namespace O21Toolbox
{
    public class O21ToolboxSettings : ModSettings
    {
        // General
        public bool animationsEnabled = true;
        public bool modUpdates = true;
        public bool alienLeathers = true;
        public float humanSpawnWeight = 20;

        // Jetpacks
        public bool roofPunch = true;
        public bool jetpackAutoRefuel = true;

        // UpdateListings
        public List<string> markedAsSeen = new List<String>();

        // Backgrounds
        //public string background = "BackgroundDef_Default";

        public override void ExposeData()
        {
            // General
            Scribe_Values.Look(ref this.animationsEnabled, "animationsEnabled", true);
            Scribe_Values.Look(ref this.modUpdates, "modUpdates", true);
            Scribe_Values.Look(ref this.alienLeathers, "alienLeathers", true);
            Scribe_Values.Look(ref this.humanSpawnWeight, "humanSpawnWeight", 20);

            // Jetpacks
            Scribe_Values.Look(ref this.roofPunch, "roofPunch", true);
            Scribe_Values.Look(ref this.jetpackAutoRefuel, "jetpackAutoRefuel", true);

            // UpdateListings
            Scribe_Collections.Look(ref this.markedAsSeen, "markedAsSeen");

            // Backgrounds
            //Scribe_Values.Look(ref background, "background", null);
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }
}
