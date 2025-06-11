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

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
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

            LogUtil.Message($"{CurrentVersion} ::");

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
            listing.LabelBacked("Mod Update Settings", Color.white);
            if(listing.ButtonTextLabeled("Unmarks all updates you've previously marked as seen, letting you see them again.", "Unmark All Updates"))
            {
                settings.markedAsSeen = new List<string>();
                UpdateUtil.allUpdatesCached = new List<UpdateDef>();
            }
            listing.CheckboxLabeled("Show Old Mod Updates", ref settings.showOldUpdates, "By default, mod updates older than three months are hidden automatically, if you enable this, you'll see them regardless of how old they are.");
            listing.Gap();
            listing.LabelBacked("Incident Settings", Color.white);
            listing.CheckboxLabeled("Special Occasions", ref settings.specialOccasions, "Some features may only happen during specific special occasions, like April Fools, so this option exists so those can be disabled.");
            listing.Gap();
            if (ModLister.BiotechInstalled)
            {
                listing.LabelBacked("Biotech Specific", Color.white);
                listing.CheckboxLabeled("Show Xenotype Editor Debug Option", ref settings.showXenotypeEditorMenu, "If Enabled, an icon will be displayed at the top of the screen along with other DevMode icons as long as DevMode is enabled, allowing access to the Xenotype editor from places other than the starter pawn loadout screen.");
                listing.Gap();
            }
            if (ModLister.RoyaltyInstalled)
            {
                listing.LabelBacked("Royalty Specific", Color.white);
                listing.CheckboxLabeled("Prevent Empire Hostility", ref settings.preventEmpireHostility, "If Enabled, automatically patches any player faction defs not be always hostile with the Empire from Royalty. Having to do them manually every time because Ludeon chose to make it that way is bullshit and I'm sick of it.");
                if (settings.preventEmpireHostility)
                {
                    listing.GapLine();
                    listing.Note("The following is a list of player faction defs, these can be toggled so they they are not affected by the faction hostility change in the case where a player faction is intended to be hostile towards them.");
                    foreach (FactionDef faction in DefDatabase<FactionDef>.AllDefs.Where(f => f.isPlayer))
                    {
                        bool bufferBool = settings.empireHostilityFixedFactions[faction.defName];
                        listing.CheckboxLabeled(faction.LabelCap, ref bufferBool);
                        settings.empireHostilityFixedFactions[faction.defName] = bufferBool;
                    }
                }
            }
        }
    }

    public enum TabulaRasaSettingsPage
    {
        General,
        Race_Spawning
    }
}
