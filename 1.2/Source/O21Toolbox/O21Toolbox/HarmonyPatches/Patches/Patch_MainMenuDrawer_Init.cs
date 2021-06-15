using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Background;

namespace O21Toolbox.HarmonyPatches
{

    //[HarmonyPatch(typeof(MainMenuDrawer), "Init")]
    //public static class Patch_MainMenuDrawer_Init
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix()
    //    {
    //        if (O21ToolboxMod.settings.background == null)
    //        {
    //            O21ToolboxMod.settings.background = DefDatabase<BackgroundDef>.GetNamed("BackgroundDef_Default");
    //        }

    //        if (O21ToolboxMod.settings.background.defName == "BackgroundDef_Default")
    //        {
    //            Background.Background.SetDefault();
    //        }
    //        else
    //        {
    //            Background.Background.AdjustBackgroundArt(O21ToolboxMod.settings.background);
    //        }
    //    }
    //}
}
