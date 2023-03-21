using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class UpdateUtil
    {
        public static void DoUpdateListing()
        {
            List<UpdateDef> allUpdates = DefDatabase<UpdateDef>.AllDefsListForReading;

            if (UI.screenHeight < 768 || UI.screenWidth < 1366 || !TabulaRasaMod.settings.modUpdates || allUpdates.NullOrEmpty())
            {
                return;
            }

            allUpdates.SortBy(x => x.date);
            allUpdates.Reverse();

            if (!TabulaRasaMod.settings.markedAsSeen.NullOrEmpty())
            {
                for (int i = 0; i < TabulaRasaMod.settings.markedAsSeen.Count(); i++)
                {
                    if (allUpdates.Any(x => x.defName == TabulaRasaMod.settings.markedAsSeen[i]))
                    {
                        allUpdates.Remove(DefDatabase<UpdateDef>.GetNamed(TabulaRasaMod.settings.markedAsSeen[i]));
                    }
                }
            }

            int selectedUpdate = -1;

            float height = 500;
            float width = 300;

            Rect rect = new Rect(8, (UI.screenHeight - (height + 120)), width, height);
            Widgets.DrawWindowBackground(rect);
            Rect inRect = rect.ContractedBy(16f);
            float curY = 0;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.font = GameFont.Medium;
            listing.Label("Mod Updates");
            curY += Text.CalcHeight("Mod Updates", inRect.width);
            listing.font = GameFont.Small;

            listing.GapLine();
            curY += 12;

            int updateCount = 6;
            if (allUpdates.Count() < 6)
            {
                updateCount = allUpdates.Count();
            }

            for (int i = 0; i < updateCount; i++)
            {
                Rect listRect = new Rect(0, curY, inRect.width, 64);
                Rect hoverRect = new Rect(listRect);
                DoUpdateSelection(listRect, allUpdates[i]);
                if (Mouse.IsOver(hoverRect))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                    {
                        RemoveSelection(allUpdates[i].defName);
                    }
                    else if (Widgets.ButtonInvisible(hoverRect))
                    {
                        Application.OpenURL(allUpdates[i].linkUrl);
                    }
                    selectedUpdate = i;
                }
                curY += 68;
            }
            listing.End();

            MainMenuDrawer.DoExpansionIcons();

            if (selectedUpdate < 0)
            {
                return;
            }
            DoSelectedUpdateInfo(allUpdates[selectedUpdate]);
        }

        public static void RemoveSelection(string defName)
        {
            if (TabulaRasaMod.settings.markedAsSeen.NullOrEmpty())
            {
                TabulaRasaMod.settings.markedAsSeen = new List<string>();
            }
            TabulaRasaMod.settings.markedAsSeen.Add(defName);
            TabulaRasaMod.mod.WriteSettings();
        }

        public static void DoUpdateSelection(Rect rect, UpdateDef info)
        {
            if (info.important)
            {
                Widgets.DrawWindowBackgroundTutor(rect);
            }
            else
            {
                Widgets.DrawWindowBackground(rect);
            }
            Rect inRect = rect.ContractedBy(8);
            Listing_Standard listing = new Listing_Standard();

            listing.Begin(inRect);

            listing.font = GameFont.Medium;
            listing.Label(info.modContentPack.Name);
            listing.font = GameFont.Small;

            string[] dateParts = info.date.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string flippedDate = dateParts[2] + "-" + dateParts[1] + "-" + dateParts[0];
            listing.Label(flippedDate);

            listing.End();
        }

        public static void DoSelectedUpdateInfo(UpdateDef info)
        {
            float height = 500;
            float width = 500;

            float curY = 0;

            Rect rect = new Rect(316, (UI.screenHeight - (height + 120)), width, height);
            Widgets.DrawWindowBackground(rect);
            Rect inRect = rect.ContractedBy(16);
            GUI.BeginGroup(inRect);

            if (!info.banner.NullOrEmpty())
            {
                Texture2D bannerImage = ContentFinder<Texture2D>.Get(info.banner, false);
                float pWidth = bannerImage?.width ?? 0;
                float pHeight = bannerImage?.height ?? 0;
                Rect bannerRect = new Rect((inRect.width - pWidth) / 2, 0, pWidth, pHeight);
                if (bannerImage != null)
                {
                    GUI.DrawTexture(bannerRect, bannerImage);
                }
                curY += 56;
            }

            Rect textRect = new Rect(0, curY, inRect.width, inRect.height);
            Widgets.Label(textRect, info.content);

            GUI.EndGroup();
        }
    }
}
