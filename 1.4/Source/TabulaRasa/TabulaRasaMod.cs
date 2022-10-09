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
        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        public TabulaRasaMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<TabulaRasaSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            Harmony harmonyTR = new Harmony("Neronix17.TabulaRasa.RimWorld");
            harmonyTR.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Tabula Rasa";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            listing.SettingsDropdown("Current Page", "", ref currentPage, listing.ColumnWidth);
            listing.Note("You will need to restart the game changing any settings as some code is only run on startup.", GameFont.Tiny);
            listing.GapLine();

            if (currentPage == TabulaRasaSettingsPage.General)
            {
                DoSettings_General(listing);
            }
            else if (currentPage == TabulaRasaSettingsPage.Race_Spawning)
            {
                DoSettings_RaceSpawning(listing);
            }
        }

        public void DoSettings_General(Listing_Standard listingStandard)
        {
            listingStandard.CheckboxLabeled("Special Occasions", ref settings.specialOccasions, "Some features may only happen during specific special occasions, like April Fools, so this option exists so those can be disabled.");
            if (ModLister.RoyaltyInstalled)
            {
                listingStandard.CheckboxLabeled("Prevent Empire Hostility", ref settings.preventEmpireHostility, "If Enabled, automatically patches any player faction defs not be always hostile with the Empire from Royalty. Having to do them manually because Ludeon fucked up is bullshit and I'm sick of it.");
            }
        }

        public void DoSettings_RaceSpawning(Listing_Standard listingStandard)
        {
            List<RaceSpawningDef> allSpawnDefs = DefDatabase<RaceSpawningDef>.AllDefs.Where(rsd => TabulaRasaStartup.CheckRaceSpawningDefForFlaws(rsd)).ToList();
            if (allSpawnDefs.NullOrEmpty())
            {
                listingStandard.Note("No races using this system are loaded!", GameFont.Tiny);
            }
            else
            {
                listingStandard.Note("Weight of Humans is 100, each additional race skews the balance so you have to decide for yourself how common they are. This list does not cover Xenotypes, those are handled very differently.", GameFont.Tiny);
            }
            for (int i = 0; i < allSpawnDefs.Count; i++)
            {
                // New Line if needed
                //if (i == Mathf.CeilToInt(allSpawnDefs.Count / 2))
                //{
                //    listingStandard.NewColumn();
                //}
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
