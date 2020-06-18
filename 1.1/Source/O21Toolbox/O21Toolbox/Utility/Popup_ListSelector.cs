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
        private readonly ThingDef _current;
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
        }

        protected override void SetInitialSizeAndPosition()
        {
            var vector = Verse.UI.MousePositionOnUIInverted;
            if (vector.x + InitialSize.x > Verse.UI.screenWidth)
            {
                vector.x = Verse.UI.screenWidth - InitialSize.x;
            }
            if (vector.y + InitialSize.y - 50 > Verse.UI.screenHeight)
            {
                vector.y = Verse.UI.screenHeight - InitialSize.y - 50;
            }
            windowRect = new Rect(vector.x, vector.y + InitialSize.y - 100, 350, 500);
        }

        public override void DoWindowContents(Rect rect)
        {
            if (!rect.Contains(Event.current.mousePosition))
            {
                if (GenUI.DistFromRect(rect, Event.current.mousePosition) > 75f)
                {
                    Close(false);
                    return;
                }
            }

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

                Widgets.RadioButton(inRect.xMax - RowHeight, inRect.y, def == _current);

            }
            Widgets.EndScrollView();
        }

        private void DoItemInfo(Rect rect, ThingDef def, ref Rect inRect, ref int index)
        {
            if(index % 2 == 1)
            {
                Widgets.DrawHighlight(new Rect(inRect.xMax, inRect.yMax, rect.width, RowHeight));
            }
            Widgets.DefIcon(new Rect(inRect.xMax, inRect.yMax + 4f, IconSize, IconSize), def);
            inRect.xMax += RowHeight;
            Rect rect2 = new Rect(inRect.xMax, inRect.yMax, def.LabelCap.GetWidthCached(), RowHeight);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, def.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            inRect.xMax += rect2.width + 4f;
            if (Mouse.IsOver(rect2))
            {
                TipSignal tip = new TipSignal(() => string.Concat(new string[]
                {
                    def.LabelCap,
                    "\n\n",
                    def.description,
                    "\n\n"
                }), def.index ^ 283684);
                TooltipHandler.TipRegion(rect2, tip);
                Widgets.DrawHighlight(rect2);
            }
            index++;
        }
    }
}
