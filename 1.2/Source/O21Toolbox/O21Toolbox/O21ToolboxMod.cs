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

            Log.Message(":: Outpost 21 Toolbox - Version 1.1.0 Initialised ::");
        }

        public override string SettingsCategory() => "O21 Toolbox";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("General Settings - Some changes may require a restart.");
            listingStandard.CheckboxLabeled("Energy Need Compatibility Mode", ref settings.energyNeedCompatMode, "It's likely you won't need to touch this.");
            //listingStandard.CheckboxLabeled("Toggle Drone Tab Visibility", ref settings.showDroneTab, "Don't change this. You won't listen though, then you'll come asking why you can't define the work priority of something in a mod, and I'll make fun of you for it.");
            listingStandard.CheckboxLabeled("Moon Cycle Enabled", ref settings.moonCycleEnabled, "Disabling this WILL cause problems with mods that rely on it. But it makes it so you multiplayer using shitsticks can stop pissing and moaning about it.");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        [DebugAction("O21 Toolbox", "Trigger Next Full Moon", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void Debug_TriggerNextFullMoon()
        {
            Find.World.GetComponent<MoonCycle.WorldComponent_MoonCycle>().DebugTriggerNextFullMoon();
        }

        [DebugAction("O21 Toolbox", "Regenerate Moons", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void Debug_RegenerateMoons()
        {
            Find.World.GetComponent<MoonCycle.WorldComponent_MoonCycle>().DebugRegenerateMoons(Find.World);
        }
    }
    public class O21ToolboxSettings : ModSettings
    {
        public bool firstStartUp = true;
        public bool energyNeedCompatMode = true;
        public bool moonCycleEnabled = true;
        //public bool showDroneTab = true;

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.firstStartUp, "FirstStartUp", true, true);
            Scribe_Values.Look<bool>(ref this.energyNeedCompatMode, "EnergyNeedCompatMode", true, true);
            Scribe_Values.Look<bool>(ref this.moonCycleEnabled, "moonCycleEnabled", true, true);
            //Scribe_Values.Look<bool>(ref this.showDroneTab, "showDroneTab", true, true);
        }
    }
}
