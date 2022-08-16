using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class TabulaRasaMod : Mod
    {
        public static TabulaRasaMod mod;
        public static TabulaRasaSettings settings;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("neronix17.toolbox").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public TabulaRasaSettingsPage currentPage = TabulaRasaSettingsPage.General;
        public Vector2 scrollPosition;

        public TabulaRasaMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<TabulaRasaSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);

            Harmony OuterRimHarmony = new Harmony("Neronix17.TabulaRasa.RimWorld");
            OuterRimHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Tabula Rasa";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float secondStageHeight;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.SettingsDropdown("Current Page", "", ref currentPage, inRect.width);
            listingStandard.Note("You will need to restart the game changing any settings as some code is only run on startup.", GameFont.Tiny);
            listingStandard.GapLine();
            listingStandard.Gap(48);
            secondStageHeight = listingStandard.CurHeight;
            listingStandard.End();

            inRect.yMin = secondStageHeight;
            Rect outRect = inRect.ContractedBy(10f); float scrollRectHeight = 3000f;

            if (currentPage == TabulaRasaSettingsPage.General)
            {
                scrollRectHeight = 300f;
            }
            else if (currentPage == TabulaRasaSettingsPage.Race_Spawning)
            {
                scrollRectHeight = (float)(Mathf.CeilToInt(DefDatabase<RaceSpawningDef>.AllDefs.Where(rsd => TabulaRasaStartup.CheckRaceSpawningDefForFlaws(rsd)).Count() / 2) * 70);
            }
            else
            {
                scrollRectHeight = 3000f;
            }

            Rect rect = new Rect(0f, 0f, outRect.width - (scrollRectHeight > outRect.height ? 18f : 0), scrollRectHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect, true);
            listingStandard = new Listing_Standard();
            if (currentPage == TabulaRasaSettingsPage.Race_Spawning)
            {
                listingStandard.ColumnWidth = outRect.width / 2.1f;
            }
            else
            {
                listingStandard.ColumnWidth = outRect.width;
            }
            listingStandard.Begin(rect);

            if (currentPage == TabulaRasaSettingsPage.General)
            {
                DoSettings_General(listingStandard);
            }
            else if (currentPage == TabulaRasaSettingsPage.Race_Spawning)
            {
                DoSettings_RaceSpawning(listingStandard);
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        public void DoSettings_General(Listing_Standard listingStandard)
        {
            listingStandard.CheckboxEnhanced("Special Occasions", "Some features may only happen during specific special occasions, like April Fools, so this option exists so those can be disabled.", ref settings.specialOccasions);
        }

        public void DoSettings_RaceSpawning(Listing_Standard listingStandard)
        {
            List<RaceSpawningDef> allSpawnDefs = DefDatabase<RaceSpawningDef>.AllDefs.Where(rsd => TabulaRasaStartup.CheckRaceSpawningDefForFlaws(rsd)).ToList();
            for (int i = 0; i < allSpawnDefs.Count; i++)
            {
                // New Line if needed
                if(i == Mathf.CeilToInt(allSpawnDefs.Count / 2))
                {
                    listingStandard.NewColumn();
                }
                // Enabled or not
                bool tempBool = settings.raceSpawningSettings[allSpawnDefs[i].defName];
                listingStandard.CheckboxLabeled($"{allSpawnDefs[i].label}", ref tempBool);
                settings.raceSpawningSettings[allSpawnDefs[i].defName] = tempBool;

                // Weight setting
                float tempFloat = settings.raceSpawningWeights[allSpawnDefs[i].defName];
                listingStandard.AddLabeledNumericalTextField($"Spawn Weight: ", ref tempFloat, 0.5f, 0f, 10000f);
                settings.raceSpawningWeights[allSpawnDefs[i].defName] = tempFloat;

                // Gap line to split for the next one
                listingStandard.GapLine();
            }
        }
    }

    public enum TabulaRasaSettingsPage
    {
        General,
        Race_Spawning
    }
}
