using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Harmony;


namespace O21Toolbox
{
    public static class O21ToolboxModSettings
    {
        public static O21ToolboxSettings settings;
    }

    [StaticConstructorOnStartup]
    public class O21ToolboxMod : Mod
    {
        public static O21ToolboxSettings settings;

        public Vector2 scrollPos = new Vector2();

        public O21ToolboxMod(ModContentPack content) : base(content)
        {
            Log.Message(":: Outpost 21 Toolbox Version 0.1.0 Loaded ::");

            settings = GetSettings<O21ToolboxSettings>();

            if (settings.FirstStartUp)
            {
                settings.ResetToDefault();
            }
        }

        public override string SettingsCategory() => "O21 Toolbox";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float yOff = 25f;
            Widgets.DrawLine(new Vector2(inRect.width / 2, inRect.y - 7f), new Vector2(inRect.width / 2, inRect.height + 75f), Color.gray, 1f);
            Rect winRect = new Rect(0f, yOff, inRect.width, inRect.height);
            Rect leftSide = new Rect(0f, winRect.y, winRect.width / 2, winRect.height);

            //GUI.BeginGroup(leftSide);
            MakeTitle(new Rect(5f, leftSide.y + 7f, leftSide.width, 17f), "ToolboxSettingsGeneralLabel".Translate());
            Rect rect1 = new Rect(0f, 20f, leftSide.width, 50).ContractedBy(5f);
            MakeNewCheckBox(rect1, "EnergyNeedCompatModeLabel".Translate(), ref settings.EnergyNeedCompatMode, out rect1, "EnergyNeedCompatModeDesc".Translate(), false, yOff);
            //GUI.EndGroup();

            if (Widgets.ButtonText(new Rect(0f, inRect.height + 35f, 125f, 45f), "Reset Default"))
            {
                settings.ResetToDefault();
            }
        }

        public void MakeNewCheckBox(Rect rect, string label, ref bool checkOn, out Rect newRect, string desc = null, bool disabled = false, float yOffset = 0f, float xOffset = 0f)
        {
            Rect rect2 = rect;
            rect2.y += yOffset;
            rect2.x += xOffset;
            Widgets.CheckboxLabeled(rect2.ContractedBy(10f), label, ref checkOn, disabled);
            if (desc != null)
            {
                TooltipHandler.TipRegion(rect2.ContractedBy(10f), desc);
            }
            newRect = rect2;
        }

        public void MakeTitle(Rect rect, string label)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect, label);
            Widgets.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin + rect.width, rect.yMax), Color.gray, 1f);
            Text.Font = GameFont.Small;
        }
    }
}
