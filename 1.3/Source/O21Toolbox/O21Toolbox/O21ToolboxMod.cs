using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;
using System.IO;

//using O21Toolbox.Background;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public class O21ToolboxMod : Mod
    {
        public static O21ToolboxMod mod;
        public static O21ToolboxSettings settings;

        public O21ToolboxSettingsPage currentPage = O21ToolboxSettingsPage.General;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("neronix17.toolbox").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public O21ToolboxMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<O21ToolboxSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"Version: {CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);
        }

        public override string SettingsCategory() => "Outpost 21 Toolbox";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float secondStageHeight;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.SettingsDropdown<O21ToolboxSettingsPage>("Current Page", "", ref currentPage, inRect.width);
            listingStandard.GapLine();
            listingStandard.Gap(48);
            secondStageHeight = listingStandard.CurHeight;
            listingStandard.End();

            listingStandard = new Listing_Standard
            {
                ColumnWidth = (inRect.width - 30f / 2f - 2f)
            };
            inRect.yMin = secondStageHeight;
            listingStandard.Begin(inRect);

            if(currentPage == O21ToolboxSettingsPage.General)
            {
                listingStandard.CheckboxEnhanced("Animations Enabled", "If animations that use this framework are causing performance issues for you, you can disable all animation code in the Toolbox with this.", ref settings.animationsEnabled);
                listingStandard.GapLine();
                listingStandard.CheckboxEnhanced("Enable Alien Leathers", "For those who don't use any alien races making use of this option, you can disable this. When RimWorld 1.4 comes around this will be automated, it's manual to prevent breaking peoples saves right now. Obviously this can and will cause errors if you disable them while using any alien races which make use of them, or on an existing save.", ref settings.alienLeathers);
                listingStandard.GapLine();
                string humanSpawnWeight_Buffer = settings.humanSpawnWeight.ToString();
                listingStandard.TextFieldNumericLabeled("Human Spawn Weight", ref settings.humanSpawnWeight, ref humanSpawnWeight_Buffer);
                //listingStandard.GapLine();
                //Func<List<FloatMenuOption>> bgOptionsMaker = delegate()
                //{
                //    List<FloatMenuOption> bgList = new List<FloatMenuOption>();
                //    List<BackgroundDef> defList = DefDatabase<BackgroundDef>.AllDefsListForReading;
                //    for (int i = 0; i < defList.Count; i++)
                //    {
                //        bgList.Add(new FloatMenuOption(defList[i].label, delegate ()
                //        {
                //            settings.background = defList[i].defName;
                //        }));
                //    }
                //    if (!bgList.Any())
                //    {
                //        bgList.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                //    }

                //    return bgList;
                //};
                //if (listingStandard.ButtonTextLabeled("Background Image", Background.Background.allBackgroundDefs.Find(d => d.defName == settings.background).label))
                //{
                //    Find.WindowStack.Add(new FloatMenu(bgOptionsMaker()));
                //}

                //Background.Background.AdjustBackgroundArt(settings.background);
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public void DoQuarrySettings(Listing_Standard listingStandard, Rect inRect)
        {

        }
    }

    public enum O21ToolboxSettingsPage
    {
        General
    }

    [StaticConstructorOnStartup]
    public static class O21ToolboxStartup
    {
        static O21ToolboxStartup()
        {
        }
    }
}
