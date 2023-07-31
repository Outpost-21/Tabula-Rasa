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
    public static class UpdateUtil
    {
        public static UpdateDef selectedUpdate;
        public static Vector2 updateScrollPosition;
        public static float updateViewRectHeight;

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
            if (allUpdates.Any(u => u.contentList.NullOrEmpty()))
            {
                allUpdates.RemoveAll(u => u.contentList.NullOrEmpty());
            }

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

            int updateCount = Mathf.Min(6, allUpdates.Count());

            for (int i = 0; i < updateCount; i++)
            {
                Rect listRect = new Rect(0, curY, inRect.width, 64);
                Rect hoverRect = new Rect(listRect);
                DoUpdateSelection(listRect, allUpdates[i], Mouse.IsOver(hoverRect));
                curY += 68;
            }
            listing.End();

            MainMenuDrawer.DoExpansionIcons();

            if(selectedUpdate != null)
            {
                DoSelectedUpdateInfo(selectedUpdate);
            }
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

        public static void DoUpdateSelection(Rect rect, UpdateDef info, bool highlight = false)
        {
            if (info.important)
            {
                Widgets.DrawWindowBackgroundTutor(rect);
            }
            else
            {
                Widgets.DrawWindowBackground(rect);
                if(highlight || selectedUpdate == info)
                {
                    GUI.DrawTexture(rect, TexUI.HighlightTex);
                }
            }
            if (Widgets.ButtonInvisible(rect))
            {
                selectedUpdate = info;
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

            Rect rect = new Rect(316, (UI.screenHeight - (height + 120)), width, height);
            Widgets.DrawWindowBackground(rect);
            Rect inRect = rect.ContractedBy(16);
            inRect.y += 12f;
            inRect.height -= 12f;
            if (CloseButtonFor(rect))
            {
                selectedUpdate = null;
            }
            if (TrashButtonFor(rect))
            {
                RemoveSelection(info.defName);
                selectedUpdate = null;
            }
            DoLinkIcons(rect, info);

            bool flag = updateViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), updateViewRectHeight);
            Widgets.BeginScrollView(inRect, ref updateScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect scrollRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(scrollRect);
            // ============================ CONTENTS ================================
            if (!info.banner.NullOrEmpty())
            {
                Texture2D bannerImage = ContentFinder<Texture2D>.Get(info.banner, false);
                listing.DoImage(bannerImage);
                listing.GapLine();
            }
            if (!info.content.NullOrEmpty())
            {
                // Do Legacy Update
                DoLegacyUpdateContents(listing, info);
            }
            if (!info.contentList.NullOrEmpty())
            {
                // Do New Style Update
                DoUpdateContents(listing, info);
            }
            // ======================================================================
            updateViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public static void DoLinkIcons(Rect rect, UpdateDef info)
        {
            if (info.links.NullOrEmpty()) { return; }
            List<UpdateLink> links = info.links ?? new List<UpdateLink>();
            if (!info.linkUrl.NullOrEmpty()) { links.Add(new UpdateLink() { linkUrl = info.linkUrl }); }
            for (int i = 0; i < links.Count; i++)
            {
                UpdateLink link = links[i];
                if (!link.linkUrl.NullOrEmpty())
                {
                    Texture2D icon = link.linkTex.NullOrEmpty() ? TexTabulaRasa.Hyperlink : ContentFinder<Texture2D>.Get(link.linkTex, false);
                    Rect iconRect = new Rect(rect.x + rect.width - (22f * (3 + i)), rect.y + 4, 18f, 18f);
                    if (DoLinkButton(iconRect, icon, link.linkLabel))
                    {
                        Application.OpenURL(link.linkUrl);
                    }
                }
            }
        }

        public static bool DoLinkButton(Rect rect, Texture2D icon, string tooltip = null)
        {
            if(tooltip != null)
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
            return Widgets.ButtonImage(rect, icon);
        }

        public static void DoLegacyUpdateContents(Listing_Standard listing, UpdateDef info)
        {
            listing.Note(info.content);
        }

        public static void DoUpdateContents(Listing_Standard listing, UpdateDef info)
        {
            foreach(UpdateItem item in info.contentList)
            {
                if(!item.header.NullOrEmpty())
                {
                    listing.LabelBacked(item.header, Color.white, GameFont.Small);
                }
                if (!item.text.NullOrEmpty())
                {
                    listing.Note(item.text);
                }
                if (!item.image.NullOrEmpty())
                {
                    Texture2D image = ContentFinder<Texture2D>.Get(item.image, false);
                    listing.DoImage(image);
                }
            }
        }

        public static bool CloseButtonFor(Rect rect)
        {
            return DoLinkButton(new Rect(rect.x + rect.width - 18f - 4f, rect.y + 4, 18f, 18f), TexButton.CloseXSmall, "Close");
        }

        public static bool TrashButtonFor(Rect rect)
        {
            return DoLinkButton(new Rect(rect.x + rect.width - 36f - 8f, rect.y + 4, 18f, 18f), TexTabulaRasa.UpdateMarkAsRead, "Mark as Read");
        }
    }
}
