using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;
using static Verse.MaterialAllocator;

namespace TabulaRasa
{
    [StaticConstructorOnStartup]
    public static class SubcategoryUtil
    {
        public const string orderCat = "Orders";
        public const string uncatCat = "Uncategorized";
        public const float gizmoScale = 75f;
        public const float gizmoMargin = 6f;
        public const float sectionBorder = 10f;
        public const float scrollWidth = 16f;
        public static float orderGizmoScale = TabulaRasaMod.settings.enableShrunkOrders ? (gizmoScale * 0.66f) : gizmoScale;
        public static Dictionary<ArchitectCategoryTab, string> currentCatForTab = new Dictionary<ArchitectCategoryTab, string>();
        public static Dictionary<ArchitectCategoryTab, Vector2> scrollPositionForTab;
        public static Dictionary<ArchitectCategoryTab, Vector2> gizmoScrollPositionForTab;
        public static Dictionary<ArchitectCategoryTab, Vector2> nbdScrollPositionForTab;
        public static Dictionary<ArchitectCategoryTab, List<ArchitectSubCatDesignators>> designatorsForTab;

        public static void PopulateArchitectCategoryTab(ArchitectCategoryTab tab)
        {
            if (designatorsForTab.NullOrEmpty()) 
            { 
                designatorsForTab = new Dictionary<ArchitectCategoryTab, List<ArchitectSubCatDesignators>>(); 
            }
            if (!designatorsForTab.ContainsKey(tab))
            {
                Dictionary<string, List<Designator>> keyedDesignators = new Dictionary<string, List<Designator>>();
                foreach (Designator d in tab.def.ResolvedAllowedDesignators)
                {
                    if (d is Designator_Build)
                    {
                        DefModExt_SubcategoryDisplay modExt = ((Designator_Build)d).PlacingDef.GetModExtension<DefModExt_SubcategoryDisplay>();
                        if (modExt != null)
                        {
                            foreach(string s in modExt.subcategories)
                            {
                                if (!keyedDesignators.ContainsKey(s))
                                {
                                    keyedDesignators.Add(s, new List<Designator>());
                                }
                                keyedDesignators[s].Add(d);
                            }
                        }
                        else
                        {
                            if (!keyedDesignators.ContainsKey(uncatCat))
                            {
                                keyedDesignators.Add(uncatCat, new List<Designator>());
                            }
                            keyedDesignators[uncatCat].Add(d);
                        }
                    }
                    else if (d is Designator_Dropdown)
                    {
                        Designator des = ((Designator_Dropdown)d).activeDesignator;
                        DefModExt_SubcategoryDisplay modExt;
                        if (des is Designator_Build)
                        {
                            modExt = ((Designator_Build)des).PlacingDef.GetModExtension<DefModExt_SubcategoryDisplay>();
                        }
                        else if (des is Designator_Place)
                        {
                            modExt = ((Designator_Place)des).PlacingDef.GetModExtension<DefModExt_SubcategoryDisplay>();
                        }
                        else { continue; }
                        if (modExt != null)
                        {
                            foreach (string s in modExt.subcategories)
                            {
                                if (!keyedDesignators.ContainsKey(s))
                                {
                                    keyedDesignators.Add(s, new List<Designator>());
                                }
                                keyedDesignators[s].Add(d);
                            }
                        }
                        else
                        {
                            if (!keyedDesignators.ContainsKey(uncatCat))
                            {
                                keyedDesignators.Add(uncatCat, new List<Designator>());
                            }
                            keyedDesignators[uncatCat].Add(d);
                        }
                    }
                    else
                    {
                        if (!keyedDesignators.ContainsKey(orderCat))
                        {
                            keyedDesignators.Add(orderCat, new List<Designator>());
                        }
                        keyedDesignators[orderCat].Add(d);
                    }
                }
                List<ArchitectSubCatDesignators> designatorsList = new List<ArchitectSubCatDesignators>();
                foreach (KeyValuePair<string, List<Designator>> kvp in keyedDesignators)
                {
                    designatorsList.Add(new ArchitectSubCatDesignators() { category = kvp.Key, designators = kvp.Value });
                }
                designatorsForTab.Add(tab, designatorsList);
            }
            PopulateTabLists(tab);
        }

        public static void PopulateTabLists(ArchitectCategoryTab tab)
        {
            if (currentCatForTab.NullOrEmpty()) { currentCatForTab = new Dictionary<ArchitectCategoryTab, string>(); }
            if (!currentCatForTab.ContainsKey(tab)) { currentCatForTab.Add(tab, default(string)); }
            if (gizmoScrollPositionForTab.NullOrEmpty()) { gizmoScrollPositionForTab = new Dictionary<ArchitectCategoryTab, Vector2>(); }
            if (!gizmoScrollPositionForTab.ContainsKey(tab)) { gizmoScrollPositionForTab.Add(tab, default(Vector2)); }
            if (nbdScrollPositionForTab.NullOrEmpty()) { nbdScrollPositionForTab = new Dictionary<ArchitectCategoryTab, Vector2>(); }
            if (!nbdScrollPositionForTab.ContainsKey(tab)) { nbdScrollPositionForTab.Add(tab, default(Vector2)); }
            if (scrollPositionForTab.NullOrEmpty()) { scrollPositionForTab = new Dictionary<ArchitectCategoryTab, Vector2>(); }
            if (!scrollPositionForTab.ContainsKey(tab)) { scrollPositionForTab.Add(tab, default(Vector2)); }
        }

        public static void DrawSubcategoryWindow(ArchitectCategoryTab tab, Designator forceActivatedCommand)
        {
            float windowWidth = UI.screenWidth - 180f - (((MainTabWindow_Architect)MainButtonDefOf.Architect.TabWindow).RequestedTabSize.x);
            float windowHeight = ((gizmoScale + gizmoMargin) * TabulaRasaMod.settings.gizmoRowHeight) + (gizmoMargin * 4f) + sectionBorder;
            Rect outRect = new Rect(((MainTabWindow_Architect)MainButtonDefOf.Architect.TabWindow).RequestedTabSize.x + 10f, 
                UI.screenHeight - windowHeight - 45f, windowWidth, windowHeight);
            Widgets.DrawWindowBackground(outRect);
            Text.Font = GameFont.Small;
            bool showCategories = designatorsForTab[tab].Any(c => c.Visible && c.category != uncatCat);
            bool showOnlyOrders = !showCategories && designatorsForTab[tab].Find(c => c.category == uncatCat) == null;
            float categoryRectWidth = showCategories ? 200f + scrollWidth : 0f;
            Rect categoryRect = new Rect(outRect.x, outRect.y, categoryRectWidth, outRect.height);
            List<Designator> orderDesignators;
            if (designatorsForTab[tab].Any(sc => sc.category == orderCat))
            {
                orderDesignators = designatorsForTab[tab].Find(sc => sc.category == orderCat).designators;
            }
            else
            {
                orderDesignators = new List<Designator>();
            }
            float orderRectWidth = (!orderDesignators.NullOrEmpty() && !showOnlyOrders) ? (TabulaRasaMod.settings.enableShrunkOrders ? orderGizmoScale * 2f : orderGizmoScale) + sectionBorder + scrollWidth + (gizmoMargin * 2) : 0f;
            float mainRectWidth = outRect.width - categoryRect.width - orderRectWidth;
            Rect mainRect = new Rect(categoryRect.xMax, outRect.y, mainRectWidth, outRect.height);
            if (!showOnlyOrders)
            {
                if (currentCatForTab[tab].NullOrEmpty())
                {
                    currentCatForTab[tab] = designatorsForTab[tab].OrderBy(d => d.category).Where(d => d.category != orderCat).First().category;
                }
            }
            if (showCategories)
            {
                DrawSubcategoryList(categoryRect, tab, designatorsForTab[tab].OrderBy(d => d.category).ToList());
            }
            Designator mouseoverGizmo = DrawMainDesignators(mainRect, forceActivatedCommand, tab, GetDesignatorsForShow(tab, showOnlyOrders));
            if (!orderDesignators.NullOrEmpty() && !showOnlyOrders)
            {
                Rect orderRect = new Rect(outRect.xMax - orderRectWidth - (sectionBorder / 2f), outRect.y, orderRectWidth - (sectionBorder / 2f), outRect.height);
                Designator orderMouseoverGizmo = DrawOrderDesignators(orderRect, tab, orderDesignators);
                if (orderMouseoverGizmo != null)
                {
                    mouseoverGizmo = orderMouseoverGizmo;
                }
            }
            if (mouseoverGizmo == null && Find.DesignatorManager.SelectedDesignator != null)
            {
                mouseoverGizmo = Find.DesignatorManager.SelectedDesignator;
            }
            tab.DoInfoBox(ArchitectCategoryTab.InfoRect, mouseoverGizmo);
            if (Event.current.type == EventType.MouseDown && Mouse.IsOver(mainRect))
            {
                Event.current.Use();
            }
        }

        public static void DrawSubcategoryList(Rect rect, ArchitectCategoryTab tab, List<ArchitectSubCatDesignators> subcategories)
        {
            Rect outRect = rect.ContractedBy(sectionBorder);
            float subcategoryCount = subcategories.Where(s => s.Visible).Count();
            float rowHeight = 35f;
            Rect viewRect = new Rect(outRect.x, outRect.y, outRect.width - scrollWidth, subcategoryCount * rowHeight);
            Vector2 scrollPos = scrollPositionForTab[tab];
            CaptureScrolling(outRect, viewRect, ref scrollPos);
            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
            float curY = viewRect.y;
            foreach (ArchitectSubCatDesignators subcat in subcategories)
            {
                if(!subcat.Visible)
                {
                    continue;
                }
                var rowRect = new Rect(viewRect.x, curY, viewRect.width, rowHeight - 3f);
                DrawCategoryBackground(rowRect, currentCatForTab[tab] == subcat.category);
                MouseoverSounds.DoRegion(rowRect);
                var labelRect = new Rect(rowRect.x + 8f, rowRect.y, rowRect.width - 8f, rowRect.height);

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, subcat.category);
                Text.Anchor = TextAnchor.UpperLeft;

                if (Widgets.ButtonInvisible(rowRect))
                {
                    currentCatForTab[tab] = subcat.category;
                }
                curY += rowHeight;
            }

            Widgets.EndScrollView();
            scrollPositionForTab[tab] = scrollPos;
        }

        public static Designator DrawMainDesignators(Rect rect, Designator forceActivatedCommand, ArchitectCategoryTab tab, 
            List<Designator> visibleDesignators)
        {
            if (currentCatForTab == null || visibleDesignators.NullOrEmpty())
            {
                return null;
            }
            Vector2 scrollPos = gizmoScrollPositionForTab[tab];
            Rect outRect = rect.ContractedBy(sectionBorder);
            List<Designator> designators = visibleDesignators;
            float rowHeight = gizmoScale + gizmoMargin + 5f;
            float gizmosPerRow = Mathf.FloorToInt((outRect.width - scrollWidth) / (gizmoScale + gizmoMargin));
            float rowCount = Mathf.CeilToInt((float)designators.Count() / gizmosPerRow);
            Rect viewRect = new Rect(outRect.x, outRect.y, outRect.width - scrollWidth, (rowCount * rowHeight));
            CaptureScrolling(outRect, viewRect, ref scrollPos);
            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
            Designator mouseoverGizmo = null;
            for (int i = 0; i < designators.Count(); i++)
            {
                float row = Mathf.FloorToInt(i / gizmosPerRow);
                float col = Mathf.FloorToInt(i % gizmosPerRow);
                float curX = viewRect.x + col * (gizmoScale + gizmoMargin);
                float curY = viewRect.y + row * rowHeight;
                DrawGizmo(curX, curY, designators[i], ref mouseoverGizmo);
            }

            Widgets.EndScrollView();
            gizmoScrollPositionForTab[tab] = scrollPos;
            return mouseoverGizmo;
        }

        public static Designator DrawOrderDesignators(Rect rect, ArchitectCategoryTab tab, List<Designator> designators)
        {
            Vector2 scrollPos = nbdScrollPositionForTab[tab];
            Rect outRect = rect.ContractedBy(0f, sectionBorder);
            float gizmosPerRow = Mathf.FloorToInt((outRect.width - scrollWidth) / (orderGizmoScale + gizmoMargin));
            float rowHeight = orderGizmoScale + gizmoMargin + 5f;
            float rowCount = Mathf.CeilToInt((float)designators.Count() / gizmosPerRow);
            Rect viewRect = new Rect(outRect.x, outRect.y, outRect.width - scrollWidth, rowCount * rowHeight);
            CaptureScrolling(outRect, viewRect, ref scrollPos);
            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
            Designator mouseoverGizmo = null;
            for (var i = 0; i < designators.Count; i++)
            {
                float row = Mathf.FloorToInt(i / gizmosPerRow);
                float col = Mathf.FloorToInt(i % gizmosPerRow);
                float curX = viewRect.x + col * (orderGizmoScale + gizmoMargin);
                float curY = viewRect.y + row * rowHeight;
                if (TabulaRasaMod.settings.enableShrunkOrders) { DrawOrderGizmo(curX, curY, designators[i], ref mouseoverGizmo); }
                else { DrawGizmo(curX, curY, designators[i], ref mouseoverGizmo); }
            }
            Widgets.EndScrollView();
            nbdScrollPositionForTab[tab] = scrollPos;
            return mouseoverGizmo;
        }

        public static List<Designator> GetDesignatorsForShow(ArchitectCategoryTab tab, bool showOnlyOrders)
        {
            List<Designator> designators;
            ArchitectSubCatDesignators tabDesignators = designatorsForTab[tab].Find(sc => sc.category == currentCatForTab[tab]) ?? null;

            if (showOnlyOrders)
            {
                ArchitectSubCatDesignators orderDesignators = designatorsForTab[tab].Find(sc => sc.category == orderCat) ?? null;
                if (orderDesignators != null) { designators = orderDesignators.designators.Where(d => d.Visible).ToList(); }
                else { designators = new List<Designator>(); }
            }
            else if (tabDesignators != null)
            {
                designators = tabDesignators.designators.Where(d => d.Visible).ToList();
            }
            else
            {
                designators = new List<Designator>();
            }

            return designators;
        }

        public static void DrawOrderGizmo(float curX, float curY, Designator designator, ref Designator mouseoverGizmo)
        {
            Rect gizmoRect = new Rect(curX, curY, orderGizmoScale, orderGizmoScale);
            GizmoResult result = designator.OrderGizmoOnGUI(gizmoRect, default);
            if (result.State >= GizmoState.Mouseover)
            {
                mouseoverGizmo = designator;
            }
            if (result.State == GizmoState.Interacted)
            {
                designator.ProcessInput(result.InteractEvent);
            }
            if (result.State == GizmoState.OpenedFloatMenu)
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach(FloatMenuOption o in designator.RightClickFloatMenuOptions)
                {
                    options.Add(o);
                }
                if (options.Any()) { Find.WindowStack.Add(new FloatMenu(options)); }
            }
        }
        public static GizmoResult OrderGizmoOnGUI(this Designator designator, Rect butRect, GizmoRenderParms parms)
        {
            Text.Font = GameFont.Tiny;
            Color color = Color.white;
            bool isOver = false;
            if (Mouse.IsOver(butRect))
            {
                isOver = true;
                if (!designator.disabled)
                {
                    color = GenUI.MouseoverColor;
                }
            }
            MouseoverSounds.DoRegion(butRect, SoundDefOf.Mouseover_Command);
            if (parms.highLight)
            {
                Widgets.DrawStrongHighlight(butRect.ExpandedBy(4f), null);
            }
            if (designator.disabled)
            {
                parms.lowLight = true;
            }
            Material material = parms.lowLight ? TexUI.GrayscaleGUI : null;
            GUI.color = (parms.lowLight ? Command.LowLightBgColor : color);
            GenUI.DrawTextureWithMaterial(butRect, parms.shrunk ? designator.BGTextureShrunk : designator.BGTexture, material, default(Rect));
            GUI.color = color;
            designator.DrawIcon(butRect, material, parms);
            bool steamDeckInNonKeyboardMode = false;
            GUI.color = Color.white;
            if (parms.lowLight)
            {
                GUI.color = Command.LowLightLabelColor;
            }
            Vector2 vector = parms.shrunk ? new Vector2(3f, 0f) : new Vector2(5f, 3f);
            Rect rect = new Rect(butRect.x + vector.x, butRect.y + vector.y, butRect.width - 10f, Text.LineHeight);
            if (SteamDeck.IsSteamDeckInNonKeyboardMode)
            {
                if (parms.isFirst)
                {
                    GUI.DrawTexture(new Rect(rect.x, rect.y, 21f, 21f), TexUI.SteamDeck_ButtonA);
                    if (KeyBindingDefOf.Accept.KeyDownEvent)
                    {
                        steamDeckInNonKeyboardMode = true;
                        Event.current.Use();
                    }
                }
            }
            else
            {
                KeyCode keyCode = (designator.hotKey == null) ? KeyCode.None : designator.hotKey.MainKey;
                if (keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
                {
                    Widgets.Label(rect, keyCode.ToStringReadable());
                    GizmoGridDrawer.drawnHotKeys.Add(keyCode);
                    if (designator.hotKey.KeyDownEvent)
                    {
                        steamDeckInNonKeyboardMode = true;
                        Event.current.Use();
                    }
                }
            }
            if (GizmoGridDrawer.customActivator != null && GizmoGridDrawer.customActivator(designator))
            {
                steamDeckInNonKeyboardMode = true;
            }
            if (Widgets.ButtonInvisible(butRect, true))
            {
                steamDeckInNonKeyboardMode = true;
            }
            if (!parms.shrunk)
            {
                string topRightLabel = designator.TopRightLabel;
                if (!topRightLabel.NullOrEmpty())
                {
                    Vector2 vector2 = Text.CalcSize(topRightLabel);
                    Rect position;
                    Rect rect2 = position = new Rect(butRect.xMax - vector2.x - 2f, butRect.y + 3f, vector2.x, vector2.y);
                    position.x -= 2f;
                    position.width += 3f;
                    Text.Anchor = TextAnchor.UpperRight;
                    GUI.DrawTexture(position, TexUI.GrayTextBG);
                    Widgets.Label(rect2, topRightLabel);
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                GUI.color = Color.white;
            }
            if (Mouse.IsOver(butRect) && designator.DoTooltip)
            {
                TipSignal tip = designator.Desc;
                if (designator.disabled && !designator.disabledReason.NullOrEmpty())
                {
                    tip.text += ("\n\n" + "DisabledCommand".Translate() + ": " + designator.disabledReason).Colorize(ColorLibrary.RedReadable);
                }
                tip.text += designator.DescPostfix;
                TooltipHandler.TipRegion(butRect, tip);
            }
            if (!designator.HighlightTag.NullOrEmpty() && (Find.WindowStack.FloatMenu == null || !Find.WindowStack.FloatMenu.windowRect.Overlaps(butRect)))
            {
                UIHighlighter.HighlightOpportunity(butRect, designator.HighlightTag);
            }
            Text.Font = GameFont.Small;
            if (steamDeckInNonKeyboardMode)
            {
                if (designator.disabled)
                {
                    if (!designator.disabledReason.NullOrEmpty())
                    {
                        Messages.Message("DisabledCommand".Translate() + ": " + designator.disabledReason, MessageTypeDefOf.RejectInput, false);
                    }
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                GizmoResult result;
                if (Event.current.button == 1)
                {
                    result = new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
                }
                else
                {
                    if (!TutorSystem.AllowAction(designator.TutorTagSelect))
                    {
                        return new GizmoResult(GizmoState.Mouseover, null);
                    }
                    result = new GizmoResult(GizmoState.Interacted, Event.current);
                    TutorSystem.Notify_Event(designator.TutorTagSelect);
                }
                return result;
            }
            else
            {
                if (isOver)
                {
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                return new GizmoResult(GizmoState.Clear, null);
            }
        }

        public static void DrawGizmo(float curX, float curY, Designator designator, ref Designator mouseoverGizmo)
        {
            Rect gizmoRect = new Rect(curX, curY, gizmoScale, gizmoScale);
            GizmoResult result = designator.GizmoOnGUI(gizmoRect.position, gizmoScale, default);
            if (result.State >= GizmoState.Mouseover)
            {
                mouseoverGizmo = designator;
            }
            if (result.State == GizmoState.Interacted)
            {
                designator.ProcessInput(result.InteractEvent);
            }
            if (result.State == GizmoState.OpenedFloatMenu)
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (FloatMenuOption o in designator.RightClickFloatMenuOptions)
                {
                    options.Add(o);
                }
                if (options.Any()) { Find.WindowStack.Add(new FloatMenu(options)); }
            }
        }

        public static void CaptureScrolling(Rect outRect, Rect viewRect, ref Vector2 scrollPosition)
        {
            if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(outRect))
            {
                scrollPosition.y += Event.current.delta.y * 20f;
                float num = 0f;
                float num2 = viewRect.height - outRect.height;
                if (scrollPosition.y < num)
                {
                    scrollPosition.y = num;
                }
                if (scrollPosition.y > num2)
                {
                    scrollPosition.y = num2;
                }
                Event.current.Use();
            }
        }

        public static void DrawCategoryBackground(Rect rect, bool selected)
        {
            if (selected)
            {
                GUI.color = Widgets.OptionSelectedBGFillColor;
                GUI.DrawTexture(rect, Texture2D.whiteTexture);
                GUI.color = Widgets.OptionSelectedBGBorderColor;
                Widgets.DrawBox(rect, 1);
                GUI.color = Color.white;
            }
            else
            {
                Widgets.DrawOptionUnselected(rect);
            }
            Widgets.DrawHighlightIfMouseover(rect);
        }
    }
}
