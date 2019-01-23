using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Harmony;


namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public class O21ToolboxMod : Mod
    {
        public static O21ToolboxSettings settings;

        public Vector2 scrollPos = new Vector2();

        public O21ToolboxMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<O21ToolboxSettings>();
        }

        public override string SettingsCategory() => "Outpost 21 Toolbox";
    }
}
