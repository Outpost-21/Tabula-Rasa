using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(MainMenuDrawer), "MainMenuOnGUI")]
    public static class Patch_MainMenuDrawer_MainMenuOnGUI
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            UpdateUtil.DoUpdateListing();
        }
    }
}
