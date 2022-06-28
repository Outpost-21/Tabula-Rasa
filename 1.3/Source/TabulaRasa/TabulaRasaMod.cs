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
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxEnhanced("Special Occasions", "Some features may only happen during specific special occasions, like April Fools, so this option exists so those can be disabled.", ref settings.specialOccasions);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
