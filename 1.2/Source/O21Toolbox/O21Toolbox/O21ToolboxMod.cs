using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public class O21ToolboxMod : Mod
    {
        public static O21ToolboxMod mod;
        public static O21ToolboxSettings settings;

        public O21ToolboxMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<O21ToolboxSettings>();

            Log.Message(":: Outpost 21 Toolbox - Version 1.2.0 Initialised ::");
            if (!settings.modUpdates) { Log.Message(":: Mod Updates Disabled ::"); }
        }

        public override string SettingsCategory() => "O21 Toolbox";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("General Settings");
            listingStandard.GapLine();
            listingStandard.CheckboxEnhanced("Animations Enabled", "If animations that use this framework are causing performance issues for you, you can disable all animation code in the Toolbox with this.", ref settings.animationsEnabled);
            //listingStandard.CheckboxEnhanced("Mod Updates", "Disable this and you waive any support.", ref settings.modUpdates);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
    public class O21ToolboxSettings : ModSettings
    {
        public bool animationsEnabled = true;
        public bool modUpdates = true;

        public List<string> markedAsSeen = new List<String>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.animationsEnabled, "animationsEnabled", true);
            Scribe_Values.Look(ref this.modUpdates, "modUpdates", true);

            Scribe_Collections.Look(ref this.markedAsSeen, "markedAsSeen");
        }
    }
}
