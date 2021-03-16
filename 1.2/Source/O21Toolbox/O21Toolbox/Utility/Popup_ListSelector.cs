using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public class Popup_ListSelector : Window
    {
        private readonly string _label;
        private ThingDef _current;
        private readonly List<ThingDef> _list;
        private readonly Action<ThingDef> _callback;

        private Vector2 scrollPosition;
        private const float RowHeight = 28f;
        private const float IconSize = 20f;

        public override Vector2 InitialSize => new Vector2(base.InitialSize.x, base.InitialSize.y);

        public Popup_ListSelector(string label, ThingDef current, List<ThingDef> list, Action<ThingDef> callback)
        {
            _label = label;
            _current = current;
            _list = list;
            _callback = callback;

            doCloseButton = true;
        }

        public override void SetInitialSizeAndPosition()
        {
            int windowHeight = 500;
            int windowWidth = 350;
            windowRect = new Rect(UI.screenWidth / 2 - (windowWidth / 2), UI.screenHeight / 2 - (windowHeight / 2), windowWidth , windowHeight);
        }

        public override void DoWindowContents(Rect rect)
        {
            //if (!rect.Contains(Event.current.mousePosition))
            //{
            //    if (GenUI.DistFromRect(rect, Event.current.mousePosition) > 75f)
            //    {
            //        Close(false);
            //        return;
            //    }
            //}

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, this.InitialSize.x / 2f, 40f), _label);
            Text.Font = GameFont.Small;
            Rect outRect = new Rect(rect);
            outRect.yMax -= this.CloseButSize.y;
            outRect.yMin += 44f;
            Rect inRect = new Rect(0f, 0f, outRect.width - 16f, 0);
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, inRect, true);
            int num2 = 0;
            foreach (ThingDef def in _list)
            {
                this.DoItemInfo(inRect, def, ref inRect, ref num2);


            }
            Widgets.EndScrollView();

            _callback(_current);
        }

        private void DoItemInfo(Rect rect, ThingDef def, ref Rect inRect, ref int index)
        {
            Rect itemRect = new Rect(inRect.xMin, inRect.yMin, inRect.width, RowHeight);
            //if (index % 2 == 1)
            //{
            //    Widgets.DrawHighlight(new Rect(itemRect.xMax, itemRect.yMax, itemRect.width, itemRect.height));
            //}
            Widgets.DefIcon(new Rect(itemRect.xMin, itemRect.yMin + 4f, IconSize, IconSize), def);
            if(Widgets.RadioButton(itemRect.xMax - IconSize, itemRect.yMin, def == _current))
            {
                _current = def;
            }
            Rect rect2 = new Rect(itemRect.xMin + (IconSize + 4f), itemRect.yMin, itemRect.width, itemRect.height);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, def.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            inRect.yMin += RowHeight;
            if (Mouse.IsOver(itemRect))
            {
                TipSignal tip = new TipSignal(() => string.Concat(new string[]
                {
                    def.LabelCap,
                    "\n\n",
                    def.description,
                    "\n\n"
                }), def.index ^ 283684);
                TooltipHandler.TipRegion(itemRect, tip);
                Widgets.DrawHighlight(itemRect);
            }
            index++;
        }
    }
}
