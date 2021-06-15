using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Background
{
    [StaticConstructorOnStartup]
    public static class Background
    {
        public static List<BackgroundDef> allBackgroundDefs = new List<BackgroundDef>();

        static Background()
        {
            foreach (BackgroundDef bgdef in DefDatabase<BackgroundDef>.AllDefs)
            {
                Log.Message("Background: " + bgdef.label);
                allBackgroundDefs.Add(bgdef);
            }

            if (O21ToolboxMod.settings.background == null)
            {
                O21ToolboxMod.settings.background = "BackgroundDef_Default";
            }

            if (O21ToolboxMod.settings.background == "BackgroundDef_Default")
            {
                SetDefault();
            }
            else
            {
                AdjustBackgroundArt(O21ToolboxMod.settings.background);
            }
        }

        public static void SwitchBackground(Texture2D path = null)
        {
            MainMenuDrawer.BackgroundMain.overrideBGImage = path;
        }

        public static void SetDefault()
        {
            if (ModLister.RoyaltyInstalled)
            {
                AdjustBackgroundArt("BackgroundDef_Royalty");
            }
            else
            {
                AdjustBackgroundArt("BackgroundDef_Vanilla");
            }
        }

        public static void AdjustBackgroundArt(string def = "BackgroundDef_Default")
        {
            BackgroundDef bgDef = DefDatabase<BackgroundDef>.GetNamed(def);

            if (bgDef.bgImagePath != null)
            {
                SwitchBackground(ContentFinder<Texture2D>.Get(bgDef.bgImagePath, true));
            }
        }
    }
}
