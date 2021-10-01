using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

//using O21Toolbox.Background;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public class O21ToolboxMod : Mod
    {
        public static O21ToolboxMod mod;
        public static O21ToolboxSettings settings;

        public O21ToolboxSettingsPage currentPage = O21ToolboxSettingsPage.General;

        public O21ToolboxMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<O21ToolboxSettings>();

            Log.Message(":: Outpost 21 Toolbox - Version 2.0.6 Initialised ::");
        }

        public override string SettingsCategory() => "O21 Toolbox";

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
